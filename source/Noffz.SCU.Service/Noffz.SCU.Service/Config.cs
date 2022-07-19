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
    }
}
