using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    public abstract class Filters
    {
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

        public abstract class Filter
        {
            public ValueMatchers.IValueMatcher valueMatcher;

            protected Filter(ValueMatchers.IValueMatcher valueMatcher)
            {
                this.valueMatcher = valueMatcher;
            }

            public abstract bool Matches(FilterInput input);
        }



        public abstract class ValueMatchers
        {
            private static object changeType(object value, Type conversionType)
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
                    var value = changeType(input, Value.GetType());
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
                    var valueL = changeType(input, LowerValue.GetType());
                    var valueH = changeType(input, HigherValue.GetType());
                    return LowerValue.CompareTo(valueL) <= 0 && HigherValue.CompareTo(valueH) >= 0;
                }
            }

        }
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
