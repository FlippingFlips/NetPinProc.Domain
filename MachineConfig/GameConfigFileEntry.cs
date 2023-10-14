using NetPinProc.Domain.PinProc;

namespace NetPinProc.Domain.MachineConfig
{
    /// <summary>
    /// Represents the lower-level game configuration in memory
    /// </summary>
    public class GameConfigFileEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public MachineType MachineType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumBalls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool DisplayMonitor { get; set; } = false;
    }
}
