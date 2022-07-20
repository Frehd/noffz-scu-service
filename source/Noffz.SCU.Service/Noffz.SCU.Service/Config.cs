using System.IO;
using System.Text.Json;

namespace Noffz.SCU.Service
{
    public class Config
    {
        public uint WarningCycles { get; }
        public uint ErrorCycles { get; }

        public Config(uint warningCycles, uint errorCycles)
        {
            this.WarningCycles = warningCycles;
            this.ErrorCycles = errorCycles;
        }

        public static Config ParseJson(string jsonString)
        {
            Config config = JsonSerializer.Deserialize<Config>(jsonString);

            return config;
        }

        public static Config ParseJsonFile(string jsonPath)
        {
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string jsonString = r.ReadToEnd();
                Config config = JsonSerializer.Deserialize<Config>(jsonString);
                return config;
            }
        }

        public static Config GetFallback()
        {
            return new Config(0, 0);
        }
    }
}
