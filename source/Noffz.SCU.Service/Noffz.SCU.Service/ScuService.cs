using Noffz.SCU.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    public class ScuService
    {
        public Config Config { get; set; }
        public ScuSession Scu { get; set; } = null;
        public ScuCard[] Cards { get; set; } = null;
        private IConnectionParams connectionParams;

        public ScuService(IConnectionParams c_params, Config config)
        {
            this.Config = config;
            connectionParams = c_params;
            ScuSession.EnableLogging = true;
            Scu = c_params.Connect();
        }

        public int DiscoverCards(int startCardAddress, int endCardAddress)
        {
            Cards = Scu.FindAllCards(startCardAddress, endCardAddress);

            return Cards.Length;
        }

        public RelayCheckRes.RelayCheck CheckRelayCounters(ScuCard card)
        {
            uint[] arr = card.GetAllRelaysCounter();
            if (arr.Length == 0)
            {
                Console.WriteLine($"Addressing Relays one by one! (Addr: {card.Address})");
                arr = new uint[card.NumberOfOutputChannels];
                for (int i = 0; i < card.NumberOfOutputChannels; i++)
                {
                    arr[i] = card.GetRelayCounter(i);
                }
            }
            return new RelayCheckRes.RelayCheck(arr, Config);
        }

        public RelayCheckRes CheckEveryCardsRelayCounters()
        {
            Dictionary<ScuCard, RelayCheckRes.RelayCheck> cardRelayCounts = new Dictionary<ScuCard, RelayCheckRes.RelayCheck>();
            foreach (ScuCard card in Cards)
            {
                cardRelayCounts.Add(card, CheckRelayCounters(card));
            }

            RelayCheckRes relayCheckRes = new RelayCheckRes(cardRelayCounts, Config);
            return relayCheckRes;
        }

        public ReportValues GenerateReport()
        {
            RelayCheckRes res = CheckEveryCardsRelayCounters();
            var totalCardErrors = 0;

            ReportValues reportValues = new ReportValues(
               currentTime: DateTime.Now,
               connectionType: connectionParams.GetConnectionName(),
               connectionAddress: connectionParams.GetConnectionAddress(),
               scannedCards: res.CardRelayChecks.Count,
               warningCycles: Config.WarningCycles,
               errorCycles: Config.ErrorCycles,
               totalRelayWarnings: res.TotalRelayCheck.Warning_indexes.Length,
               totalRelayErrors: res.TotalRelayCheck.Error_indexes.Length,
               totalCardErrors: 0,
               cardReports: null);

            List<CardReportValues> cardReports = new List<CardReportValues>();
            foreach (KeyValuePair<ScuCard, RelayCheckRes.RelayCheck> cardRelayCheck in res.CardRelayChecks)
            {
                ScuCard card = cardRelayCheck.Key;
                RelayCheckRes.RelayCheck cardRes = cardRelayCheck.Value;
                string[] errors = card.GetErrors();
                totalCardErrors += errors.Length;

                CardReportValues cardReport = new CardReportValues(
                    address: card.Address,
                    firmwareVersion: card.FirmwareVersion,
                    numberOfInputChannels: card.NumberOfInputChannels,
                    numberOfOutputChannels: card.NumberOfOutputChannels,
                    numberOfErrors: errors.Length,
                    errors: string.Join(",", errors),
                    relayCounts: cardRes.Counts,
                    relayWarningIndexes: cardRes.Warning_indexes,
                    relayErrorIndexes: cardRes.Error_indexes,
                    relayWarnings: cardRes.Warning_indexes.Length,
                    relayErrors: cardRes.Error_indexes.Length);

                cardReports.Add(cardReport);
            }

            reportValues.TotalCardErrors = totalCardErrors;
            reportValues.CardReports = cardReports.ToArray();

            return reportValues;
        }
    }
}
