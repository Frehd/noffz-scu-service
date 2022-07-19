using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Noffz.SCU.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"NOFFZ SCU Service Console has been run. You passed the following arguments: {String.Join(",", args)}");
            ScuService service = null;

            try
            {
                service = new ScuService(new ConnectionParams.IP("192.168.0.11"), new Config(100, 800));
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError occured while connecting! Error message: {e.Message}\nStack Trace: {e.StackTrace}");
            }


            int MinCardAddress = 0;
            int MaxCardAddress = 20;
            Console.Write($"\nSearching for installed cards in address range {MinCardAddress}-{MaxCardAddress}...");
            service.discoverCards(0, 20);
            Console.WriteLine($"Found {service.Cards.Length} device(s).");

            if (service.Cards.Length == 0)
            {
                throw new ApplicationException("No cards found!");
            }

            Console.WriteLine("Card info:");
            foreach (var card in service.Cards)
            {
                Console.WriteLine($"\tAddress: {card.Address}\tFirmware version: {card.FirmwareVersion}");
            }

            RelayCheckRes res = service.checkEveryCardsRelayCounters();

            foreach (var card in service.Cards)
            {
                Console.WriteLine($"\nChecking card with address: {card.Address}");
                Console.WriteLine($"\tFirmware version: {card.FirmwareVersion}");
                string[] errors = card.GetErrors();
                Console.WriteLine($"\tError count: {errors.Length}\n\tErrors: {(errors.Length == 0 ? "No errors" : String.Join(", ", errors))}");
                Console.WriteLine($"\tNumber of inputs: {card.NumberOfInputChannels}, Number of outputs: {card.NumberOfOutputChannels}");
                Console.WriteLine("\n\tRelay info:");

                Console.WriteLine($"\t\tTotal errors: {res.CardRelayChecks[card].Error_indexes.Length}, total warnings: {res.CardRelayChecks[card].Warning_indexes.Length}");
                Console.WriteLine($"\t\tRelays with warnings (>{service.Config.WarningCycles} cycles): {string.Join(",", res.CardRelayChecks[card].Warning_indexes)}");
                Console.WriteLine($"\t\tRelays with errors (>{service.Config.ErrorCycles} cycles): {string.Join(",", res.CardRelayChecks[card].Error_indexes)}");

            }

            Console.WriteLine($"\nTotal errors: {res.TotalRelayCheck.Error_indexes.Length}, total warnings: {res.TotalRelayCheck.Warning_indexes.Length}");

            Console.WriteLine("Done!");
        }
    }
}
