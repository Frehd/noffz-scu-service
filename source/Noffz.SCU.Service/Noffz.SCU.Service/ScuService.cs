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
    }
}
