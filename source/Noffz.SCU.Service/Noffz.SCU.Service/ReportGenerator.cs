using CsvHelper;
using Noffz.SCU.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    public struct ReportValues
    {

        public string CurrentTime { get; set; }
        public string ConnectionType { get; set; }
        public string ConnectionAddress { get; set; }
        public int ScannedCards { get; set; }
        public uint WarningCycles { get; set; }
        public uint ErrorCycles { get; set; }
        public int TotalRelayWarnings { get; set; }
        public int TotalRelayErrors { get; set; }
        public int TotalCardErrors { get; set; }

        public ReportValues(
            string currentTime,
            string connectionType,
            string connectionAddress,
            int scannedCards,
            uint warningCycles,
            uint errorCycles,
            int totalRelayWarnings,
            int totalRelayErrors,
            int totalCardErrors,
            CardReportValues[] cardReports)
        {
            CurrentTime = currentTime;
            ConnectionType = connectionType;
            ConnectionAddress = connectionAddress;
            ScannedCards = scannedCards;
            WarningCycles = warningCycles;
            ErrorCycles = errorCycles;
            TotalRelayWarnings = totalRelayWarnings;
            TotalRelayErrors = totalRelayErrors;
            TotalCardErrors = totalCardErrors;
            CardReports = cardReports;
        }

        public CardReportValues[] CardReports { get; set; }
    }

    public struct CardReportValues
    {

        public int Address { get; set; }
        public string FirmwareVersion { get; set; }
        public int NumberOfInputChannels { get; set; }
        public int NumberOfOutputChannels { get; set; }
        public int NumberOfErrors { get; set; }
        public string Errors { get; set; }
        public uint[] RelayCounts { get; set; }
        public bool[] RelayStates { get; set; }
        public string[] RelayCycleWarningStates { get; set; }
        public int[] RelayWarningIndexes { get; set; }
        public int[] RelayErrorIndexes { get; set; }
        public int RelayWarnings { get; set; }
        public int RelayErrors { get; set; }

        public CardReportValues(
            int address,
            string firmwareVersion,
            int numberOfInputChannels,
            int numberOfOutputChannels,
            int numberOfErrors,
            string errors,
            uint[] relayCounts,
            bool[] relayStates,
            string[] relayCycleWarningStates,
            int[] relayWarningIndexes,
            int[] relayErrorIndexes,
            int relayWarnings,
            int relayErrors)
        {
            Address = address;
            FirmwareVersion = firmwareVersion;
            NumberOfInputChannels = numberOfInputChannels;
            NumberOfOutputChannels = numberOfOutputChannels;
            NumberOfErrors = numberOfErrors;
            Errors = errors;
            RelayCounts = relayCounts;
            RelayStates = relayStates;
            RelayCycleWarningStates = relayCycleWarningStates;
            RelayWarningIndexes = relayWarningIndexes;
            RelayErrorIndexes = relayErrorIndexes;
            RelayWarnings = relayWarnings;
            RelayErrors = relayErrors;
        }
    }

    struct RelayLine
    {
        public int Address { get; set; }
        public int Number { get; set; }
        public uint Cycles { get; set; }
        public bool State { get; set; }
        public uint HighLimit { get; set; }
        public uint HighHighLimit { get; set; }
        public string CycleState { get; set; }

        public RelayLine(int address, int number, uint cycles, bool state, uint highLimit, uint highHighLimit, string cycleState)
        {
            Address = address;
            Number = number;
            Cycles = cycles;
            State = state;
            HighLimit = highLimit;
            HighHighLimit = highHighLimit;
            CycleState = cycleState;
        }
    }

    public class ReportGenerator
    {
        public static void GenerateCSV(ReportValues rep)
        {
            using (var writer = new StreamWriter("report.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                GenerateGeneralCSV(csv, rep);
                GenerateCountsCSV(csv, rep);
            }
        }

        static void GenerateGeneralCSV(CsvWriter csv, ReportValues rep)
        {
            csv.WriteHeader(typeof(ReportValues));
            csv.NextRecord();
            csv.WriteRecord(rep);
            csv.NextRecord();

            csv.NextRecord();
            csv.WriteField("Card table");
            csv.NextRecord();
            csv.WriteHeader(typeof(CardReportValues));
            csv.NextRecord();
            csv.WriteRecords(rep.CardReports);
            csv.NextRecord();

        }

        static void GenerateCountsCSV(CsvWriter csv, ReportValues rep)
        {
            List<RelayLine> relays = new List<RelayLine>();
            foreach (CardReportValues card in rep.CardReports)
            {
                for (int i = 0; i < card.RelayCounts.Length; i++)
                {
                    RelayLine relayLine = new RelayLine(
                        address: card.Address,
                        number: i,
                        cycles: card.RelayCounts[i],
                        state: card.RelayStates[i],
                        highLimit: rep.WarningCycles,
                        highHighLimit: rep.ErrorCycles,
                        cycleState: card.RelayCycleWarningStates[i]);
                    relays.Add(relayLine);

                }
            }
            csv.WriteField("Relay Table");
            csv.NextRecord();
            csv.WriteHeader(typeof(RelayLine));
            csv.NextRecord();
            csv.WriteRecords(relays);
        }
    }
}
