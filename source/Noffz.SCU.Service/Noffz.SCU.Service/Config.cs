using Newtonsoft.Json;
using System;
using System.IO;
using static Noffz.SCU.Service.Filters;
using static Noffz.SCU.Service.LimitMatcher;

namespace Noffz.SCU.Service
{
    public class Config
    {
        public Rule RelayLimitFilter { get; }

        public Config(uint warningCycles, uint errorCycles, Rule relayLimitFilter)
        {
            RelayLimitFilter = relayLimitFilter;
        }

        public RelayLimit GetRelayLimit(FilterInput input)
        {
            RelayLimit? maybeLimit = RelayLimitFilter.Match(input);
            if (maybeLimit == null)
            {
                throw new ApplicationException("Couldn't match relay to limit error in config file base case!");
            }

            return maybeLimit.Value;
        }

        public static Config ParseJson(string jsonString)
        {
            Config config = JsonConvert.DeserializeObject<Config>(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return config;
        }

        public static Config ParseJsonFile(string jsonPath)
        {
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string jsonString = r.ReadToEnd();
                return ParseJson(jsonString);
            }
        }

        public static Config GetFallback()
        {
            Rule baseRule = new Rule();
            baseRule.RelayLimit = new RelayLimit(100, 800);

            Rule cardFilter = new Rule();
            baseRule.Children.Add(cardFilter);
            cardFilter.Filter = new PropertyFilters.CardAddressFilter(new ValueMatchers.Same(3));
            cardFilter.RelayLimit = new RelayLimit(1000, 8000);


            Rule relayFilter = new Rule();
            cardFilter.Children.Add(relayFilter);
            relayFilter.Filter = new PropertyFilters.RelayAddressFilter(new ValueMatchers.Range(0, 5));
            relayFilter.RelayLimit = new RelayLimit(200, 400);

            Config config = new Config(0, 0, baseRule);

            string jsonTest = JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return config;
        }
    }
}
