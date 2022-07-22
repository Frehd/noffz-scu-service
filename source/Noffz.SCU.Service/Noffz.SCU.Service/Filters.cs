using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    public abstract class Filters
    {
        /// <summary>
        /// The information provided for matching a relay to a <c>RelayLimit</c>.
        /// </summary>
        public struct FilterInput
        {
            public string CardName { get; set; }
            public int CardAddress { get; set; }
            public int RelayAddress { get; set; }

            public FilterInput(string cardName, int cardAddress, int relayAddress)
            {
                CardName = cardName;
                CardAddress = cardAddress;
                RelayAddress = relayAddress;
            }
        }

        /// <summary>
        /// A <c>Filter</c> decides wether or not a rule applies to the given <c>FilterInput</c>
        /// </summary>
        public abstract class Filter
        {
            public ValueMatchers.IValueMatcher valueMatcher;

            protected Filter(ValueMatchers.IValueMatcher valueMatcher)
            {
                this.valueMatcher = valueMatcher;
            }

            public abstract bool Matches(FilterInput input);
        }


        /// <summary>
        /// A <c>ValueMatcher</c> is used in a <c>Filter</c> to match a property of a <c>FilterInput</c>.
        /// </summary>
        public abstract class ValueMatchers
        {
            private static object ChangeType(object value, Type conversionType)
            {
                return Convert.ChangeType(value, conversionType);
            }

            public interface IValueMatcher
            {
                bool Matches(object input);
            }

            public class Same : IValueMatcher
            {
                public Same(object value)
                {
                    Value = value;
                }

                public object Value { get; set; }
                public bool Matches(object input)
                {
                    var value = ChangeType(input, Value.GetType());
                    return Value.Equals(value);
                }
            }

            public class Range : IValueMatcher
            {
                public Range(IComparable lowerValue, IComparable higherValue)
                {
                    LowerValue = lowerValue;
                    HigherValue = higherValue;
                }

                public IComparable LowerValue { get; set; }
                public IComparable HigherValue { get; set; }
                public bool Matches(object input)
                {
                    var valueL = ChangeType(input, LowerValue.GetType());
                    var valueH = ChangeType(input, HigherValue.GetType());
                    return LowerValue.CompareTo(valueL) <= 0 && HigherValue.CompareTo(valueH) >= 0;
                }
            }

        }

        /// <summary>
        /// A <c>Filter</c> decides wether or not a rule applies to the given <c>FilterInput</c>
        /// </summary>
        public abstract class PropertyFilters
        {
            public class CardNameFilter : Filter
            {

                public CardNameFilter(ValueMatchers.IValueMatcher valueMatcher) : base(valueMatcher)
                {
                }

                override public bool Matches(FilterInput input)
                {
                    return valueMatcher.Matches(input.CardName);
                }
            }

            public class CardAddressFilter : Filter
            {
                public CardAddressFilter(ValueMatchers.IValueMatcher valueMatcher) : base(valueMatcher)
                {
                }

                override public bool Matches(FilterInput input)
                {
                    return valueMatcher.Matches(input.CardAddress);
                }
            }

            public class RelayAddressFilter : Filter
            {
                public RelayAddressFilter(ValueMatchers.IValueMatcher valueMatcher) : base(valueMatcher)
                {
                }

                override public bool Matches(FilterInput input)
                {
                    return valueMatcher.Matches(input.RelayAddress);
                }
            }
        }
    }
}
