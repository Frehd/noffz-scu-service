using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Noffz.SCU.Service.Filters;

namespace Noffz.SCU.Service
{

    public abstract class LimitMatcher
    {

        /// <summary>
        /// Defines under which conditions the given <c>RelayLimit</c> is applied.
        /// </summary>
        public class Rule
        {
            /// <summary>
            /// Defines the necessary conditions in order for the <c>Rule</c> and its children to be invoked.
            /// When set to <c>null</c> this rule is always invoked.
            /// </summary>
            public Filter Filter { get; set; }

            /// <summary>
            /// Defines the further rules that are dependent on this rule and override it.
            /// The first matching rule will always be applied without backtracking.
            /// </summary>
            public List<Rule> Children { get; set; } = new List<Rule>();

            /// <summary>
            /// The <c>RelayLimit</c> that will be returned in case this <c>Rule</c> is matched and there are no further matching children.
            /// </summary>
            public RelayLimit RelayLimit { get; set; }

            /// <summary>
            /// Matches a given <c>FilterInput</c> to a <c>RelayLimit</c>.
            /// </summary>
            /// <param name="input">The <c>FilterInput</c> to be matched to.</param>
            /// <returns>The matching <c>RelayLimit</c> or null in case of this rule not matching.</returns>
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

        /// <summary>
        /// Describes the number of switching cycles a relay can go through before being flagged as either 'Warning' or 'Error'.
        /// </summary>
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
