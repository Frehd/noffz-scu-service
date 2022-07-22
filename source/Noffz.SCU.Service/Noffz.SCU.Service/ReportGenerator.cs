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
        public int NumberOfCardsWithErrors { get; set; }
        public int TotalNumberOfRelayWarnings { get; set; }
        public int TotalNumberOfRelayErrors { get; set; }
        public int TotalNumberOfCardControllerErrors { get; set; }

        public ReportValues(
            string reportTime,
            string connectionType,
            string connectionAddress,
            int numberOfScannedCards,
            int numberOfCardsWithErrors,
            int totalNumberOfRelayWarnings,
            int totalNumberOfRelayErrors,
            int totalNumberOfCardControllerErrors,
            CardReportValues[] cardReports)
        {
            ReportTime = reportTime;
            ConnectionType = connectionType;
            ConnectionAddress = connectionAddress;
            NumberOfScannedCards = numberOfScannedCards;
            NumberOfCardsWithErrors = numberOfCardsWithErrors;
            TotalNumberOfRelayWarnings = totalNumberOfRelayWarnings;
            TotalNumberOfRelayErrors = totalNumberOfRelayErrors;
            TotalNumberOfCardControllerErrors = totalNumberOfCardControllerErrors;
            CardReports = cardReports;
        }

        public CardReportValues[] CardReports { get; set; }

        public static string GetHtmlTableHeader()
        {
            return @"<tr>
                <th> ReportTime </th>
                <th> ConnectionType </th>
                <th> ConnectionAddress </th>
                <th> NumberOfScannedCards </th>
                <th> numberOfCardsWithErrors </th>
                <th> TotalNumberOfRelayWarnings </th>
                <th> TotalNumberOfRelayErrors </th>
                <th> TotalNumberOfCardControllerErrors </th>
            </th> ";
        }
        public string GetHtml()
        {
            return $@"<tr>
                <th> {ReportTime} </th>
                <th> {ConnectionType} </th>
                <th> {ConnectionAddress} </th>
                <th> {NumberOfScannedCards} </th>
                <th> {NumberOfCardsWithErrors} </th>
                <th> {TotalNumberOfRelayWarnings} </th>
                <th> {TotalNumberOfRelayErrors} </th>
                <th> {TotalNumberOfCardControllerErrors} </th>
            </th> ";
        }
    }

    public struct CardReportValues
    {

        public int CardAddress { get; set; }
        public string FirmwareVersion { get; set; }
        public int NumberOfInputChannels { get; set; }
        public int NumberOfOutputChannels { get; set; }
        public int NumberOfControllerErrors { get; set; }
        public string ControllerErrors { get; set; }
        public bool[] RelayStates { get; set; }
        public uint[] RelayCounts { get; set; }
        public uint[] WarningLimits { get; set; }
        public uint[] ErrorLimits { get; set; }
        public WarningErrorState[] RelayCycleWarningStates { get; set; }
        public int[] RelayWarningIndexes { get; set; }
        public int[] RelayErrorIndexes { get; set; }
        public int RelayWarnings { get; set; }
        public int RelayErrors { get; set; }
        public WarningErrorState CardStatus { get; set; }

        public CardReportValues(
            int cardAddress,
            string firmwareVersion,
            int numberOfInputChannels,
            int numberOfOutputChannels,
            int numberOfControllerErrors,
            string controllerErrors,
            bool[] relayStates,
            uint[] relayCounts,
            uint[] warningLimits,
            uint[] errorLimits,
            WarningErrorState[] relayCycleWarningStates,
            int[] relayWarningIndexes,
            int[] relayErrorIndexes,
            int relayWarnings,
            int relayErrors,
            WarningErrorState cardStatus)
        {
            CardAddress = cardAddress;
            FirmwareVersion = firmwareVersion;
            NumberOfInputChannels = numberOfInputChannels;
            NumberOfOutputChannels = numberOfOutputChannels;
            NumberOfControllerErrors = numberOfControllerErrors;
            ControllerErrors = controllerErrors;
            RelayStates = relayStates;
            RelayCounts = relayCounts;
            WarningLimits = warningLimits;
            ErrorLimits = errorLimits;
            RelayCycleWarningStates = relayCycleWarningStates;
            RelayWarningIndexes = relayWarningIndexes;
            RelayErrorIndexes = relayErrorIndexes;
            RelayWarnings = relayWarnings;
            RelayErrors = relayErrors;
            CardStatus = cardStatus;
        }

        public static string GetHtmlTableHeader()
        {
            return @"<tr>
                <th> CardAddress </th>
                <th> FirmwareVersion </th>
                <th> NumberOfInputChannels </th>
                <th> NumberOfOutputChannels </th>
                <th> NumberOfControllerErrors </th>
                <th> ControllerErrors </th>
                <th> RelayWarnings </th>
                <th> RelayErrors </th>
                <th> CardStatus </th>
            </th> ";
        }
        public string GetHtml()
        {
            return $@"<tr>
                <th> {CardAddress} </th>
                <th> {FirmwareVersion} </th>
                <th> {NumberOfInputChannels} </th>
                <th> {NumberOfOutputChannels} </th>
                <th> {NumberOfControllerErrors} </th>
                <th> {ControllerErrors} </th>
                <th> {RelayWarnings} </th>
                <th> {RelayErrors} </th>
                <th class='{CardStatus.ToString().ToLower()}'> {CardStatus} </th>
            </th> ";
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
        public WarningErrorState RelayStatus { get; set; }

        public RelayLine(int cardAddress, int relayNumber, bool relayState, uint relayCycles, uint warningLimit, uint errorLimit, WarningErrorState relayStatus)
        {
            CardAddress = cardAddress;
            RelayNumber = relayNumber;
            RelayState = relayState;
            RelayCycles = relayCycles;
            WarningLimit = warningLimit;
            ErrorLimit = errorLimit;
            RelayStatus = relayStatus;
        }

        public static string GetHtmlTableHeader()
        {
            return @"<tr>
                <th> CardAddress </th>
                <th> RelayNumber </th>
                <th> RelayState </th>
                <th> RelayCycles </th>
                <th> WarningLimit </th>
                <th> ErrorLimit </th>
                <th> RelayStatus </th>
            </th> ";
        }
        public string GetHtml()
        {
            return $@"<tr>
                <th> {CardAddress} </th>
                <th> {RelayNumber} </th>
                <th> {RelayState} </th>
                <th> {RelayCycles} </th>
                <th> {WarningLimit} </th>
                <th> {ErrorLimit} </th>
                <th class='{RelayStatus.ToString().ToLower()}'> {RelayStatus} </th>
            </th> ";
        }
    }

    public class ReportGenerator
    {
        public static void GenerateCSV(ReportValues rep, string path, string fileName)
        {
            using (var writer = new StreamWriter(path+fileName))
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
            csv.WriteField("Card Table");
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
                        warningLimit: card.WarningLimits[i],
                        errorLimit: card.ErrorLimits[i],
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

        public static void GenerateHTML(ReportValues rep, string path, string fileName)
        {

            StringBuilder cardTableString = new StringBuilder();
            StringBuilder relayTableString = new StringBuilder();
            foreach (CardReportValues cardReport in rep.CardReports)
            {
                cardTableString.Append(cardReport.GetHtml());

                for (int i = 0; i < cardReport.RelayCounts.Length; i++)
                {
                    RelayLine relayLine = new RelayLine(
                        cardAddress: cardReport.CardAddress,
                        relayNumber: i,
                        relayState: cardReport.RelayStates[i],
                        relayCycles: cardReport.RelayCounts[i],
                        warningLimit: cardReport.WarningLimits[i],
                        errorLimit: cardReport.ErrorLimits[i],
                        relayStatus: cardReport.RelayCycleWarningStates[i]);

                    relayTableString.Append(relayLine.GetHtml());
                }
            }

            string page =
$@"<!DOCTYPE html>
<html>
<head>
    <style>
        .warning {{
            background-color: yellow;
        }}
        .error {{
            background-color: red;
        }}
        :is(h1, h2, h3, h4, h5, h6) {{
            font-family: arial, sans-serif;
        }}
        table {{
            font-family: arial, sans-serif;
            border-collapse: collapse;
            width: 100%;
            width:0.1%;
            white-space: nowrap;
        }}

        td, th {{
            border: 1px solid #dddddd;
            text-align: left;
            padding: 8px;
        }}

        tr:nth-child(even) {{
            background-color: #dddddd;
        }}
    </style>
</head>
<body>

    <h2>Noffz.SCU.Service Report</h2>

    <table>
        {ReportValues.GetHtmlTableHeader()}
        {rep.GetHtml()}
    </table>

    <h3>Card Table</h3>

    <table>
        {CardReportValues.GetHtmlTableHeader()}
        {cardTableString}
    </table>

    <h3>Relay Table</h3>

    <table>
        {RelayLine.GetHtmlTableHeader()}
        {relayTableString}
    </table>

</body>
</html>";

            using (var writer = new StreamWriter(path+fileName))
            {
                writer.Write(page);
            }
        }
    }
}
