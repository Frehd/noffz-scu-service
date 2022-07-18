namespace Noffz.SCU.Service
{
    public class Config
    {
        public uint warningCycles { get; }
        public uint errorCycles { get; }

        public Config(uint warningCycles, uint errorCycles)
        {
            this.warningCycles = warningCycles;
            this.errorCycles = errorCycles;
        }
    }
}
