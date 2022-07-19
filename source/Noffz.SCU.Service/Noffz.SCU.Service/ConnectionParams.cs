using System;

namespace Noffz.SCU.Service
{
    public abstract class ConnectionParams
    {
        public class COMPort : ConnectionParams
        {
            public Int32 ComPortNumber;

            public COMPort(int comPortNumber)
            {
                this.ComPortNumber = comPortNumber;
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
