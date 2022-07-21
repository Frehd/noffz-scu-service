using CommandLine;
using Noffz.SCU.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Noffz.SCU.Service
{
    class Program
    {

        class Options
        {
            [Option('i', "interface", Required = true, HelpText = "\"lan\"/\"com\" The interface for the connection.")]
            public string Interface { get; set; }

            [Option('a', "addr", Required = true, HelpText = "The IP-Address or COMPort number.")]
            public string Address { get; set; }

            [Option('c', "config", Required = true, HelpText = "Path to config file.")]
            public string ConfigPath { get; set; }

            [Option('r', "report", Default = false, HelpText = "Generate a report.")]
            public bool GenerateReport { get; set; }

            [Option('o', "out", Default = "", HelpText = "Output path for report.")]
            public string ReportPath { get; set; }

            [Option("card-range", Default = "0-20", HelpText = "Card Address range to be scanned.")]
            public string CardAddressRange { get; set; }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => RunOptions(opts, args));

        }

        static void RunOptions(Options opts, string[] args)
        {
            Console.WriteLine($"NOFFZ SCU Service Console has been run. You passed the following arguments: {String.Join(",", args)}");
            ScuService service = null;

            if (opts.Interface != "lan" && opts.Interface != "com")
            {
                throw new ApplicationException($"Interface option not recognized: {opts.Interface}");
            }

            Config conf;
            try
            {
                conf = Config.ParseJsonFile(opts.ConfigPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError occured while parsing config file! Error message: {e.Message}\nStack Trace: {e.StackTrace}");
                Console.WriteLine($"Proceeding with default settings...");
                conf = Config.GetFallback();
            }


            try
            {
                if (opts.Interface == "lan")
                {
                    var factory = new IPConnectionParamsCreator(opts.Address);
                    var con = factory.Create();
                    service = new ScuService(con, conf);
                }
                else if (opts.Interface == "com")
                {
                    var factory = new COMPortConnectionParamsCreator(int.Parse(opts.Address));
                    var con = factory.Create();
                    service = new ScuService(con, conf);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError occured while connecting! Error message: {e.Message}\nStack Trace: {e.StackTrace}");
            }


            int MinCardAddress = 0;
            int MaxCardAddress = 20;

            if (!String.IsNullOrEmpty(opts.CardAddressRange))
            {
                try
                {
                    var range = opts.CardAddressRange.Split('-').Select(x => int.Parse(x)).ToArray();
                    MinCardAddress = range.Min();
                    MaxCardAddress = range.Max();
                }
                catch
                {
                    Console.WriteLine("Unable to parse your answer. Default scan range will be used.");
                }
            }

            Console.Write($"\nSearching for installed cards in address range {MinCardAddress}-{MaxCardAddress}...");
            service.DiscoverCards(MinCardAddress, MaxCardAddress);
            Console.WriteLine($"Found {service.Cards.Length} device(s).");

            if (service.Cards.Length == 0)
            {
                throw new ApplicationException("No cards found!");
            }

            Console.WriteLine("Card info:");
            foreach (ScuCard card in service.Cards)
            {
                Console.WriteLine($"\tAddress: {card.Address}\tFirmware version: {card.FirmwareVersion}");
            }

            Console.WriteLine("Reading card info...");
            ReportValues rep = service.GenerateReport();

            foreach (CardReportValues card in rep.CardReports)
            {
                Console.WriteLine($"\nChecking card with address: {card.CardAddress}");
                Console.WriteLine($"\tFirmware version: {card.FirmwareVersion}");
                Console.WriteLine($"\tError count: {card.NumberOfControllerErrors}\n\tErrors: {(card.NumberOfControllerErrors == 0 ? "No errors" : card.ControllerErrors)}");
                Console.WriteLine($"\tNumber of inputs: {card.NumberOfInputChannels}, Number of outputs: {card.NumberOfOutputChannels}");
                Console.WriteLine("\n\tRelay info:");

                Console.WriteLine($"\t\tTotal errors: {card.RelayErrors}, total warnings: {card.RelayWarnings}");
                Console.WriteLine($"\t\tRelays with warnings: {string.Join(",", card.RelayWarningIndexes)}");
                Console.WriteLine($"\t\tRelays with errors: {string.Join(",", card.RelayErrorIndexes)}");


            }

            Console.WriteLine($"\nTotal relay errors: {rep.TotalNumberOfRelayErrors}, total relay warnings: {rep.TotalNumberOfRelayWarnings}");
            Console.WriteLine($"Total card controller errors: {rep.TotalNumberOfCardControllerErrors}");

            if (opts.GenerateReport)
            {
                Console.WriteLine("Generating CSVs");
                ReportGenerator.GenerateCSV(rep);
            }

            Console.WriteLine("Done!");
        }
    }
}
