using Noffz.SCU.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    public class RelayCheckRes
    {
        public class RelayCheck
        {
            public uint[] counts { get; }
            public int[] warning_indexes;
            public int[] error_indexes;

            public RelayCheck(uint[] counts, Config config)
            {
                this.counts = counts;
                var indexed_counts = counts.Select((value, index) => new { value, index });

                warning_indexes = indexed_counts.Where(c => c.value >= config.warningCycles && c.value < config.errorCycles).Select(c => c.index).ToArray();
                error_indexes = indexed_counts.Where(c => c.value >= config.errorCycles).Select(c => c.index).ToArray();

            }
        }

        public Dictionary<ScuCard, RelayCheck> cardRelayChecks { get; } = new Dictionary<ScuCard, RelayCheck>();
        public RelayCheck totalRelayCheck { get; }

        public RelayCheckRes(Dictionary<ScuCard, uint[]> cardRelayCounts, Config config)
        {
            List<uint> allCounts = new List<uint>();

            foreach (var card in cardRelayCounts)
            {
                allCounts.AddRange(card.Value);

                RelayCheck relayCheck = new RelayCheck(card.Value, config);
                cardRelayChecks.Add(card.Key, relayCheck);
            }

            totalRelayCheck = new RelayCheck(allCounts.ToArray(), config);
        }

        public RelayCheckRes(Dictionary<ScuCard, RelayCheck> cardRelayCounts, Config config)
        {
            List<uint> allCounts = new List<uint>();

            foreach (var cardCheck in cardRelayCounts)
            {
                allCounts.AddRange(cardCheck.Value.counts);

                cardRelayChecks.Add(cardCheck.Key, cardCheck.Value);
            }

            totalRelayCheck = new RelayCheck(allCounts.ToArray(), config);
        }
    }
}
