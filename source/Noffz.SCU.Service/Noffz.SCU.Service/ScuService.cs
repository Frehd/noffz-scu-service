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
            Console.Write($"Reading cards relay info (Addr: {card.Address})");
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
                Console.Write($"\tAddressing Relays one by one!");
                cnt_arr = new uint[card.NumberOfOutputChannels];
                state_arr = new bool[card.NumberOfOutputChannels];
                for (int i = 0; i < card.NumberOfOutputChannels; i++)
                {
                    cnt_arr[i] = card.GetRelayCounter(i);
                    state_arr[i] = card.GetRelayState(i);
                }
            }
            Console.WriteLine("");
            return new RelayCheckRes.RelayCheck(cnt_arr, state_arr, card, Config);
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
               numberOfCardsWithErrors: 0,
               totalNumberOfRelayWarnings: res.TotalRelayCheck.WarningIndexes.Length,
               totalNumberOfRelayErrors: res.TotalRelayCheck.ErrorIndexes.Length,
               totalNumberOfCardControllerErrors: 0,
               cardReports: null);

            List<CardReportValues> cardReports = new List<CardReportValues>();
            foreach (KeyValuePair<ScuCard, RelayCheckRes.RelayCheck> cardRelayCheck in res.CardRelayChecks)
            {
                ScuCard card = cardRelayCheck.Key;
                RelayCheckRes.RelayCheck cardRes = cardRelayCheck.Value;
                string[] errors = card.GetErrors();
                totalCardControllerErrors += errors.Length;
                string cardStatus = "OK";
                if (errors.Length != 0 || cardRes.ErrorIndexes.Length != 0)
                {
                    numberOfCardsWithErrors++;
                    cardStatus = "Error";
                }
                else if (cardRes.WarningIndexes.Length != 0)
                {
                    cardStatus = "Warning";
                }

                CardReportValues cardReport = new CardReportValues(
                    cardAddress: card.Address,
                    firmwareVersion: card.FirmwareVersion,
                    numberOfInputChannels: card.NumberOfInputChannels,
                    numberOfOutputChannels: card.NumberOfOutputChannels,
                    numberOfControllerErrors: errors.Length,
                    controllerErrors: string.Join(",", errors),
                    relayStates: cardRes.States,
                    relayCounts: cardRes.Counts,
                    warningLimits: cardRes.WarningLimits,
                    errorLimits: cardRes.ErrorLimits,
                    relayCycleWarningStates: cardRes.CycleWarningStates,
                    relayWarningIndexes: cardRes.WarningIndexes,
                    relayErrorIndexes: cardRes.ErrorIndexes,
                    relayWarnings: cardRes.WarningIndexes.Length,
                    relayErrors: cardRes.ErrorIndexes.Length,
                    cardStatus: cardStatus);

                cardReports.Add(cardReport);
            }

            reportValues.TotalNumberOfCardControllerErrors = totalCardControllerErrors;
            reportValues.NumberOfCardsWithErrors = numberOfCardsWithErrors;
            reportValues.CardReports = cardReports.ToArray();

            return reportValues;
        }
    }
}
