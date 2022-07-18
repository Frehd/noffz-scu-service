using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noffz.SCU.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"NOFFZ SCU Service Console has been run. You passed the following arguments: {String.Join(",",args)}");
            var service = new ScuService(new ConnectionParams.IP("192.168.0.1"), new Config(100_000, 150_000));
        }
    }
}
