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
            public uint[] Counts { get; }
            public int[] Warning_indexes;
            public int[] Error_indexes;

            public RelayCheck(uint[] counts, Config config)
            {
                this.Counts = counts;
                var indexed_counts = counts.Select((value, index) => new { value, index });

                Warning_indexes = indexed_counts.Where(c => c.value >= config.WarningCycles && c.value < config.ErrorCycles).Select(c => c.index).ToArray();
                Error_indexes = indexed_counts.Where(c => c.value >= config.ErrorCycles).Select(c => c.index).ToArray();

            }
        }

        public Dictionary<ScuCard, RelayCheck> CardRelayChecks { get; } = new Dictionary<ScuCard, RelayCheck>();
        public RelayCheck TotalRelayCheck { get; }

        public RelayCheckRes(Dictionary<ScuCard, uint[]> cardRelayCounts, Config config)
        {
            List<uint> allCounts = new List<uint>();

            foreach (var card in cardRelayCounts)
            {
                allCounts.AddRange(card.Value);

                RelayCheck relayCheck = new RelayCheck(card.Value, config);
                CardRelayChecks.Add(card.Key, relayCheck);
            }

            TotalRelayCheck = new RelayCheck(allCounts.ToArray(), config);
        }

        public RelayCheckRes(Dictionary<ScuCard, RelayCheck> cardRelayCounts, Config config)
        {
            List<uint> allCounts = new List<uint>();

            foreach (var cardCheck in cardRelayCounts)
            {
                allCounts.AddRange(cardCheck.Value.Counts);

                CardRelayChecks.Add(cardCheck.Key, cardCheck.Value);
            }

            TotalRelayCheck = new RelayCheck(allCounts.ToArray(), config);
        }
    }
}
