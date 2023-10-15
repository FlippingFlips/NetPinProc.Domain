#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace NetPinProc.Domain.MachineConfig
{
    public class PRDriverGlobalsEntry
    {
        public ushort LampMatrixStrobeTime { get; set; } = 200;
        public ushort WatchdogTime { get; set; } = 1000;
        public bool UseWatchdog { get; set; } = true;
    }
}
