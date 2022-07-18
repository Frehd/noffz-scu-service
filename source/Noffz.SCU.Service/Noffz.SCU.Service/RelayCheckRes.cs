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
            public int errors;
            public int warnings;

            public RelayCheck(uint[] counts, Config config)
            {
                this.counts = counts;
                warnings = counts.Where(c => c > config.warningCycles).Count();
                errors = counts.Where(c => c > config.errorCycles).Count();

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
    }
}
