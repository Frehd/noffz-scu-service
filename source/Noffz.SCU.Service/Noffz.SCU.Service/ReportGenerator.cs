using Noffz.SCU.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    public struct ReportValues
    {

        public DateTime CurrentTime;
        public int ScannedCards;
        public uint WarningCycles;
        public uint ErrorCycles;
        public int TotalRelayWarnings;
        public int TotalRelayErrors;

        public ReportValues(
            DateTime currentTime,
            int scannedCards,
            uint warningCycles,
            uint errorCycles,
            int totalRelayWarnings,
            int totalRelayErrors,
            CardReportValues[] cardReports)
        {
            CurrentTime = currentTime;
            ScannedCards = scannedCards;
            WarningCycles = warningCycles;
            ErrorCycles = errorCycles;
            TotalRelayWarnings = totalRelayWarnings;
            TotalRelayErrors = totalRelayErrors;
            CardReports = cardReports;
        }

        public CardReportValues[] CardReports { get; set; }
    }

    public struct CardReportValues
    {


        public int Address;
        public string FirmwareVersion;
        public int NumberOfInputChannels;
        public int NumberOfOutputChannels;
        public int NumberOfErrors;
        public string Errors;
        public uint[] RelayCounts;
        public int[] RelayWarningIndexes;
        public int[] RelayErrorIndexes;
        public int RelayWarnings;
        public int RelayErrors;

        public CardReportValues(
            int address,
            string firmwareVersion,
            int numberOfInputChannels,
            int numberOfOutputChannels,
            int numberOfErrors,
            string errors,
            uint[] relayCounts,
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
            RelayWarningIndexes = relayWarningIndexes;
            RelayErrorIndexes = relayErrorIndexes;
            RelayWarnings = relayWarnings;
            RelayErrors = relayErrors;
        }
    }

    public class ReportGenerator
    {
        public void GenerateReport(RelayCheckRes res, Config config)
        {
            ReportValues reportValues = new ReportValues(
                DateTime.Now,
                res.CardRelayChecks.Count,
                config.WarningCycles,
                config.ErrorCycles,
                res.TotalRelayCheck.Warning_indexes.Length,
                res.TotalRelayCheck.Error_indexes.Length,
                null);

            List<CardReportValues> cardReports = new List<CardReportValues>();
            foreach (KeyValuePair<ScuCard, RelayCheckRes.RelayCheck> cardRelayCheck in res.CardRelayChecks)
            {
                ScuCard card = cardRelayCheck.Key;
                RelayCheckRes.RelayCheck cardRes = cardRelayCheck.Value;
                string[] errors = card.GetErrors();

                CardReportValues cardReport = new CardReportValues(
                    card.Address,
                    card.FirmwareVersion,
                    card.NumberOfInputChannels,
                    card.NumberOfOutputChannels,
                    errors.Length,
                    string.Join(",", errors),
                    cardRes.Counts,
                    cardRes.Warning_indexes,
                    cardRes.Error_indexes,
                    cardRes.Warning_indexes.Length,
                    cardRes.Error_indexes.Length);
                cardReports.Add(cardReport);
            }

            reportValues.CardReports = cardReports.ToArray();

        }
    }
}
