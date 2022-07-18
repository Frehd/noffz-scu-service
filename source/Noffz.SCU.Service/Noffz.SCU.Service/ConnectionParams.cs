using System;

namespace Noffz.SCU.Service
{
    public abstract class ConnectionParams
    {
        public class COMPort : ConnectionParams
        {
            public Int32 comPortNumber;

            public COMPort(int comPortNumber)
            {
                this.comPortNumber = comPortNumber;
            }
        }
        public class IP : ConnectionParams
        {
            public string IPAddress;

            public IP(string iPAddress)
            {
                IPAddress = iPAddress;
            }
        }
    }
}
