using Newtonsoft.Json;
using System;
using System.IO;
using static Noffz.SCU.Service.Filters;
using static Noffz.SCU.Service.LimitMatcher;

namespace Noffz.SCU.Service
{

    /// <summary>
    /// Represents the values saved in the user provided JSON config file.
    /// </summary>
    public class Config
    {
        public Rule RelayLimitFilter { get; }

        public Config(Rule relayLimitFilter)
        {
            RelayLimitFilter = relayLimitFilter;
        }

        /// <summary>
        /// Provides the relevant <c>RelayLimit</c> for a specified Relay.
        /// </summary>
        /// <param name="input">The relevant <c>FilterInput</c>.</param>
        /// <returns>The matched <c>RelayLimit</c>.</returns>
        /// <exception cref="ApplicationException"></exception>
        public RelayLimit GetRelayLimit(FilterInput input)
        {
            RelayLimit? maybeLimit = RelayLimitFilter.Match(input);
            if (maybeLimit == null)
            {
                throw new ApplicationException("Couldn't match relay to limit error in config file base case!");
            }

            return maybeLimit.Value;
        }

        /// <summary>
        /// Parses a <c>Config</c> object from a JSON string.
        /// </summary>
        /// <param name="jsonString">The JSON string to be parsed.</param>
        /// <returns>A <c>Config</c> object parsed from the JSON string.</returns>
        public static Config ParseJson(string jsonString)
        {
            Config config = JsonConvert.DeserializeObject<Config>(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return config;
        }

        /// <summary>
        /// Parses a <c>Config</c> object from a JSON file.
        /// </summary>
        /// <param name="jsonPath">The path to the JSON file.</param>
        /// <returns>A <c>Config</c> object parsed from the JSON file.</returns>
        public static Config ParseJsonFile(string jsonPath)
        {
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string jsonString = r.ReadToEnd();
                return ParseJson(jsonString);
            }
        }

        /// <summary>
        /// Provides a default <c>Config</c> object in case no other <c>Config</c> object is available.
        /// </summary>
        /// <returns>A defautl <c>Config</c> object.</returns>
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

            Config config = new Config(baseRule);

            string jsonTest = JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return config;
        }
    }
}
