using Noffz.SCU.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    public class ScuService
    {
        public Config config { get; set; }
        public ScuSession scu { get; set; } = null;
        public ScuCard[] cards { get; set; } = null;

        public ScuService(ConnectionParams c_params, Config config)
        {
            this.config = config;
            ScuSession.EnableLogging = true;
            connect(c_params);
        }

        private void connect(ConnectionParams c_params)
        {
            switch (c_params)
            {
                case ConnectionParams.COMPort comport:
                    scu = new ScuSession(comport.comPortNumber);
                    break;
                case ConnectionParams.IP ip:
                    scu = new ScuSession(ip.IPAddress);
                    break;
            }
        }

        public int discoverCards(int startCardAddress, int endCardAddress)
        {
            cards = scu.FindAllCards(startCardAddress, endCardAddress);

            return cards.Length;
        }

        public RelayCheckRes.RelayCheck checkRelayCounters(ScuCard card)
        {
            uint[] arr = card.GetAllRelaysCounter();
            if (arr.Length == 0)
            {
                Console.WriteLine($"Addressing Relays one by one! (Addr: {card.Address})");
                arr = new uint[card.NumberOfOutputChannels];
                for (int i = 0; i < card.NumberOfOutputChannels; i++)
                {
                    arr[i] = card.GetRelayCounter(i);
                }
            }
            return new RelayCheckRes.RelayCheck(arr, config);
        }
        public RelayCheckRes checkEveryCardsRelayCounters()
        {
            Dictionary<ScuCard, RelayCheckRes.RelayCheck> cardRelayCounts = new Dictionary<ScuCard, RelayCheckRes.RelayCheck>();
            foreach (ScuCard card in cards)
            {
                cardRelayCounts.Add(card, checkRelayCounters(card));
            }

            RelayCheckRes relayCheckRes = new RelayCheckRes(cardRelayCounts, config);
            return relayCheckRes;
        }
    }
}
