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
        public Config Config { get; set; }
        public ScuSession Scu { get; set; } = null;
        public ScuCard[] Cards { get; set; } = null;

        public ScuService(ConnectionParams c_params, Config config)
        {
            this.Config = config;
            ScuSession.EnableLogging = true;
            connect(c_params);
        }

        private void connect(ConnectionParams c_params)
        {
            switch (c_params)
            {
                case ConnectionParams.COMPort comport:
                    Scu = new ScuSession(comport.comPortNumber);
                    break;
                case ConnectionParams.IP ip:
                    Scu = new ScuSession(ip.IPAddress);
                    break;
            }
        }

        public int discoverCards(int startCardAddress, int endCardAddress)
        {
            Cards = Scu.FindAllCards(startCardAddress, endCardAddress);

            return Cards.Length;
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
            return new RelayCheckRes.RelayCheck(arr, Config);
        }
        public RelayCheckRes checkEveryCardsRelayCounters()
        {
            Dictionary<ScuCard, RelayCheckRes.RelayCheck> cardRelayCounts = new Dictionary<ScuCard, RelayCheckRes.RelayCheck>();
            foreach (ScuCard card in Cards)
            {
                cardRelayCounts.Add(card, checkRelayCounters(card));
            }

            RelayCheckRes relayCheckRes = new RelayCheckRes(cardRelayCounts, Config);
            return relayCheckRes;
        }
    }
}
