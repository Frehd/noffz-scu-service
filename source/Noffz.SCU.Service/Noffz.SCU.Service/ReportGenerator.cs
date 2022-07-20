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
        public string ConnectionType;
        public string ConnectionAddress;
        public int ScannedCards;
        public uint WarningCycles;
        public uint ErrorCycles;
        public int TotalRelayWarnings;
        public int TotalRelayErrors;
        public int TotalCardErrors;

        public ReportValues(
            DateTime currentTime,
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
}
