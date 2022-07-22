using Noffz.SCU.API;
using System;

namespace Noffz.SCU.Service
{

    public abstract class ConnectionParamsCreator
    {
        public abstract IConnectionParams Create();
    }

    public class IPConnectionParamsCreator : ConnectionParamsCreator
    {
        IP con;

        public IPConnectionParamsCreator(string ipAddr)
        {
            con = new IP(ipAddr);
        }

        public override IConnectionParams Create()
        {
            return con;
        }
    }

    public class COMPortConnectionParamsCreator : ConnectionParamsCreator
    {
        COMPort con;

        public COMPortConnectionParamsCreator(int addr)
        {
            con = new COMPort(addr);
        }

        public override IConnectionParams Create()
        {
            return con;
        }
    }

    /// <summary>
    /// Describes the parameters to connect to an SCU.
    /// </summary>
    public interface IConnectionParams
    {
        string GetConnectionName();
        string GetConnectionAddress();
        ScuSession Connect();
    }


    /// <summary>
    /// Provides the parameters to connect via COMPort to SCU.
    /// </summary>
    class COMPort : IConnectionParams
    {
        public Int32 ComPortNumber;

        public COMPort(int comPortNumber)
        {
            ComPortNumber = comPortNumber;
        }

        public ScuSession Connect()
        {
            return new ScuSession(ComPortNumber);
        }

        public string GetConnectionAddress()
        {
            return ComPortNumber.ToString();
        }

        public string GetConnectionName()
        {
            return "COM";
        }
    }

    /// <summary>
    /// Provides the parameters to connect via LAN to SCU.
    /// </summary>
    class IP : IConnectionParams
    {
        public string IPAddress;

        public IP(string iPAddress)
        {
            IPAddress = iPAddress;
        }

        public ScuSession Connect()
        {
            return new ScuSession(IPAddress);
        }

        public string GetConnectionAddress()
        {
            return IPAddress;
        }

        public string GetConnectionName()
        {
            return "IP";
        }
    }
}
