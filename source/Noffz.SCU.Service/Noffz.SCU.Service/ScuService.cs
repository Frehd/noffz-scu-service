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
        private Config config;
        private ScuSession scu = null;
        private ScuCard[] cards = null;

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

        public RelayCheckRes checkRelayCounters()
        {
            Dictionary<ScuCard, uint[]> cardRelayCounts = new Dictionary<ScuCard, uint[]>();
            foreach (ScuCard card in cards)
            {
                uint[] arr = card.GetAllRelaysCounter();
                cardRelayCounts.Add(card, arr);
            }

            RelayCheckRes relayCheckRes = new RelayCheckRes(cardRelayCounts, config);
            return relayCheckRes;
        }
    }
}
