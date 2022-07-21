using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Noffz.SCU.Service.Filters;

namespace Noffz.SCU.Service
{
    public class LimitMatcher
    {
        public class Rule
        {
            public Filter Filter { get; set; }
            public List<Rule> Children { get; set; } = new List<Rule>();
            public RelayLimit RelayLimit { get; set; }

            public RelayLimit? Match(FilterInput input)
            {
                if (Filter == null || Filter.Matches(input))
                {
                    foreach (Rule nextCondition in Children)
                    {
                        if (nextCondition.Match(input) != null)
                        {
                            return nextCondition.Match(input);
                        }
                    }
                    return RelayLimit;
                }
                return null;
            }
        }

        public struct RelayLimit
        {
            public uint WarningCycles { get; set; }
            public uint ErrorCycles { get; set; }

            public RelayLimit(uint warningCycles, uint errorCycles)
            {
                WarningCycles = warningCycles;
                ErrorCycles = errorCycles;
            }
        }
    }
}
