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

        public RelayCheckRes.RelayCheck CheckRelays(ScuCard card)
        {
            uint[] cnt_arr;
            bool[] state_arr;
            try
            {
                cnt_arr = card.GetAllRelaysCounter();
                state_arr = card.GetAllRelaysState();
            }
            catch
            {
                cnt_arr = new uint[0];
                state_arr = new bool[0];
            }


            if (cnt_arr.Length == 0)
            {
                Console.WriteLine($"Addressing Relays one by one! (Addr: {card.Address})");
                cnt_arr = new uint[card.NumberOfOutputChannels];
                state_arr = new bool[card.NumberOfOutputChannels];
                for (int i = 0; i < card.NumberOfOutputChannels; i++)
                {
                    cnt_arr[i] = card.GetRelayCounter(i);
                    state_arr[i] = card.GetRelayState(i);
                }
            }
            return new RelayCheckRes.RelayCheck(cnt_arr, state_arr, Config);
        }

        public RelayCheckRes CheckEveryCardsRelays()
        {
            Dictionary<ScuCard, RelayCheckRes.RelayCheck> cardRelayCounts = new Dictionary<ScuCard, RelayCheckRes.RelayCheck>();
            foreach (ScuCard card in Cards)
            {
                cardRelayCounts.Add(card, CheckRelays(card));
            }

            RelayCheckRes relayCheckRes = new RelayCheckRes(cardRelayCounts, Config);
            return relayCheckRes;
        }

        public ReportValues GenerateReport()
        {
            RelayCheckRes res = CheckEveryCardsRelays();
            var totalCardControllerErrors = 0;
            var numberOfCardsWithErrors = 0;

            ReportValues reportValues = new ReportValues(
               reportTime: DateTime.Now.ToString(),
               connectionType: connectionParams.GetConnectionName(),
               connectionAddress: connectionParams.GetConnectionAddress(),
               numberOfScannedCards: res.CardRelayChecks.Count,
               numberCardsWithErrors: 0,
               warningCycles: Config.WarningCycles,
               errorCycles: Config.ErrorCycles,
               totalNumberOfRelayWarnings: res.TotalRelayCheck.Warning_indexes.Length,
               totalNumberOfRelayErrors: res.TotalRelayCheck.Error_indexes.Length,
               totalNumberOfCardControllerErrors: 0,
               cardReports: null);

            List<CardReportValues> cardReports = new List<CardReportValues>();
            foreach (KeyValuePair<ScuCard, RelayCheckRes.RelayCheck> cardRelayCheck in res.CardRelayChecks)
            {
                ScuCard card = cardRelayCheck.Key;
                RelayCheckRes.RelayCheck cardRes = cardRelayCheck.Value;
                string[] errors = card.GetErrors();
                totalCardControllerErrors += errors.Length;
                bool cardOk = true;
                if (errors.Length != 0 || cardRes.Error_indexes.Length != 0)
                {
                    numberOfCardsWithErrors++;
                    cardOk = false;
                }

                CardReportValues cardReport = new CardReportValues(
                    cardAddress: card.Address,
                    firmwareVersion: card.FirmwareVersion,
                    numberOfInputChannels: card.NumberOfInputChannels,
                    numberOfOutputChannels: card.NumberOfOutputChannels,
                    numberOfControllerErrors: errors.Length,
                    controllerErrors: string.Join(",", errors),
                    relayCounts: cardRes.Counts,
                    relayStates: cardRes.States,
                    relayCycleWarningStates: cardRes.CycleWarningStates,
                    relayWarningIndexes: cardRes.Warning_indexes,
                    relayErrorIndexes: cardRes.Error_indexes,
                    relayWarnings: cardRes.Warning_indexes.Length,
                    relayErrors: cardRes.Error_indexes.Length,
                    cardStatus: cardOk?"OK":"Error");

                cardReports.Add(cardReport);
            }

            reportValues.TotalNumberOfCardControllerErrors = totalCardControllerErrors;
            reportValues.NumberCardsWithErrors = numberOfCardsWithErrors;
            reportValues.CardReports = cardReports.ToArray();

            return reportValues;
        }
    }
}
