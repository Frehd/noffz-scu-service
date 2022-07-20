using CsvHelper;
using CsvHelper.Configuration.Attributes;
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
        public string ReportTime { get; set; }
        public string ConnectionType { get; set; }
        public string ConnectionAddress { get; set; }
        public int NumberOfScannedCards { get; set; }
        public int NumberCardsWithErrors { get; set; }
        public uint WarningCycles { get; set; }
        public uint ErrorCycles { get; set; }
        public int TotalNumberOfRelayWarnings { get; set; }
        public int TotalNumberOfRelayErrors { get; set; }
        public int TotalNumberOfCardControllerErrors { get; set; }

        public ReportValues(
            string reportTime,
            string connectionType,
            string connectionAddress,
            int numberOfScannedCards,
            int numberCardsWithErrors,
            uint warningCycles,
            uint errorCycles,
            int totalNumberOfRelayWarnings,
            int totalNumberOfRelayErrors,
            int totalNumberOfCardControllerErrors,
            CardReportValues[] cardReports)
        {
            ReportTime = reportTime;
            ConnectionType = connectionType;
            ConnectionAddress = connectionAddress;
            NumberOfScannedCards = numberOfScannedCards;
            NumberCardsWithErrors = numberCardsWithErrors;
            WarningCycles = warningCycles;
            ErrorCycles = errorCycles;
            TotalNumberOfRelayWarnings = totalNumberOfRelayWarnings;
            TotalNumberOfRelayErrors = totalNumberOfRelayErrors;
            TotalNumberOfCardControllerErrors = totalNumberOfCardControllerErrors;
            CardReports = cardReports;
        }

        public CardReportValues[] CardReports { get; set; }
    }

    public struct CardReportValues
    {

        public int CardAddress { get; set; }
        public string FirmwareVersion { get; set; }
        public int NumberOfInputChannels { get; set; }
        public int NumberOfOutputChannels { get; set; }
        public int NumberOfControllerErrors { get; set; }
        public string ControllerErrors { get; set; }
        public uint[] RelayCounts { get; set; }
        public bool[] RelayStates { get; set; }
        public string[] RelayCycleWarningStates { get; set; }
        public int[] RelayWarningIndexes { get; set; }
        public int[] RelayErrorIndexes { get; set; }
        public int RelayWarnings { get; set; }
        public int RelayErrors { get; set; }
        public string CardStatus { get; set; }

        public CardReportValues(
            int cardAddress,
            string firmwareVersion,
            int numberOfInputChannels,
            int numberOfOutputChannels,
            int numberOfControllerErrors,
            string controllerErrors,
            uint[] relayCounts,
            bool[] relayStates,
            string[] relayCycleWarningStates,
            int[] relayWarningIndexes,
            int[] relayErrorIndexes,
            int relayWarnings,
            int relayErrors,
            string cardStatus)
        {
            CardAddress = cardAddress;
            FirmwareVersion = firmwareVersion;
            NumberOfInputChannels = numberOfInputChannels;
            NumberOfOutputChannels = numberOfOutputChannels;
            NumberOfControllerErrors = numberOfControllerErrors;
            ControllerErrors = controllerErrors;
            RelayCounts = relayCounts;
            RelayStates = relayStates;
            RelayCycleWarningStates = relayCycleWarningStates;
            RelayWarningIndexes = relayWarningIndexes;
            RelayErrorIndexes = relayErrorIndexes;
            RelayWarnings = relayWarnings;
            RelayErrors = relayErrors;
            CardStatus = cardStatus;
        }
    }

    struct RelayLine
    {
        public int CardAddress { get; set; }
        public int RelayNumber { get; set; }
        public bool RelayState { get; set; }
        public uint RelayCycles { get; set; }
        public uint WarningLimit { get; set; }
        public uint ErrorLimit { get; set; }
        public string RelayStatus { get; set; }

        public RelayLine(int cardAddress, int relayNumber, bool relayState, uint relayCycles, uint warningLimit, uint errorLimit, string relayStatus)
        {
            CardAddress = cardAddress;
            RelayNumber = relayNumber;
            RelayState = relayState;
            RelayCycles = relayCycles;
            WarningLimit = warningLimit;
            ErrorLimit = errorLimit;
            RelayStatus = relayStatus;
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
                        cardAddress: card.CardAddress,
                        relayNumber: i,
                        relayState: card.RelayStates[i],
                        relayCycles: card.RelayCounts[i],
                        warningLimit: rep.WarningCycles,
                        errorLimit: rep.ErrorCycles,
                        relayStatus: card.RelayCycleWarningStates[i]);
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
