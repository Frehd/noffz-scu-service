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
            public bool[] States { get; }
            public string[] CycleWarningStates { get; }
            public int[] Warning_indexes;
            public int[] Error_indexes;

            public RelayCheck(uint[] counts, bool[] states, Config config)
            {
                Counts = counts;
                States = states;
                var indexed_counts = counts.Select((value, index) => new { value, index });
                CycleWarningStates = Enumerable.Repeat("OK", counts.Length).ToArray();

                Warning_indexes = indexed_counts.Where(c => c.value >= config.WarningCycles && c.value < config.ErrorCycles).Select(c => c.index).ToArray();
                Error_indexes = indexed_counts.Where(c => c.value >= config.ErrorCycles).Select(c => c.index).ToArray();

                foreach (int error_idx in Error_indexes)
                {
                    CycleWarningStates[error_idx] = "Error";
                }
                foreach (int warning_idx in Warning_indexes)
                {
                    CycleWarningStates[warning_idx] = "Warning";
                }
            }
        }

        public Dictionary<ScuCard, RelayCheck> CardRelayChecks { get; } = new Dictionary<ScuCard, RelayCheck>();
        public RelayCheck TotalRelayCheck { get; }

        public RelayCheckRes(Dictionary<ScuCard, RelayCheck> cardRelayCounts, Config config)
        {
            List<uint> allCounts = new List<uint>();
            List<bool> allStates = new List<bool>();

            foreach (KeyValuePair<ScuCard, RelayCheck> cardCheck in cardRelayCounts)
            {
                allCounts.AddRange(cardCheck.Value.Counts);
                allStates.AddRange(cardCheck.Value.States);

                CardRelayChecks.Add(cardCheck.Key, cardCheck.Value);
            }

            TotalRelayCheck = new RelayCheck(allCounts.ToArray(), allStates.ToArray(), config);
        }
    }
}
