using Noffz.SCU.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Noffz.SCU.Service.Filters;
using static Noffz.SCU.Service.LimitMatcher;

namespace Noffz.SCU.Service
{
    public class RelayCheckRes
    {
        public class RelayCheck
        {
            public uint[] Counts { get; set; }
            public bool[] States { get; set; }
            public string[] CycleWarningStates { get; }
            public int[] WarningIndexes { get; set; }
            public int[] ErrorIndexes { get; set; }
            public uint[] WarningLimits { get; set; }
            public uint[] ErrorLimits { get; set; }

            public RelayCheck(uint[] counts, bool[] states, ScuCard card, Config config)
            {
                Counts = counts;
                States = states;
                CycleWarningStates = Enumerable.Repeat("OK", counts.Length).ToArray();
                WarningLimits = new uint[counts.Length];
                ErrorLimits = new uint[counts.Length];
                List<int> warningIndexes = new List<int>();
                List<int> errorIndexes = new List<int>();

                for (int i = 0; i < counts.Length; i++)
                {
                    FilterInput filterInput = new FilterInput(null, card.Address, i);
                    RelayLimit limit = config.GetRelayLimit(filterInput);
                    WarningLimits[i] = limit.WarningCycles;
                    ErrorLimits[i] = limit.ErrorCycles;

                    if (counts[i] >= limit.WarningCycles && counts[i] < limit.ErrorCycles)
                    {
                        CycleWarningStates[i] = "Warning";
                        warningIndexes.Add(i);
                    }
                    else if (counts[i] >= limit.ErrorCycles)
                    {
                        CycleWarningStates[i] = "Error";
                        errorIndexes.Add(i);
                    }
                }
                WarningIndexes = warningIndexes.ToArray();
                ErrorIndexes = errorIndexes.ToArray();
            }

            public RelayCheck()
            {
            }

            public RelayCheck(IEnumerable<RelayCheck> relayChecks)
            {
                List<uint> counts = new List<uint>();
                List<bool> states = new List<bool>();
                List<string> cycleWarningStates = new List<string>();
                List<int> warningIndexes = new List<int>();
                List<int> errorIndexes = new List<int>();
                List<uint> warningLimits = new List<uint>();
                List<uint> errorLimits = new List<uint>();


                foreach (RelayCheck check in relayChecks)
                {
                    counts.AddRange(check.Counts);
                    states.AddRange(check.States);
                    cycleWarningStates.AddRange(check.CycleWarningStates);
                    warningIndexes.AddRange(check.WarningIndexes);
                    errorIndexes.AddRange(check.ErrorIndexes);
                    warningLimits.AddRange(check.ErrorLimits);
                    errorLimits.AddRange(check.ErrorLimits);
                }

                Counts = counts.ToArray();
                States = states.ToArray();
                CycleWarningStates = cycleWarningStates.ToArray();
                WarningIndexes = warningIndexes.ToArray();
                ErrorIndexes = errorIndexes.ToArray();
                WarningLimits = warningLimits.ToArray();
                ErrorLimits = errorLimits.ToArray();

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

            TotalRelayCheck = new RelayCheck(CardRelayChecks.Values);
        }
    }
}
