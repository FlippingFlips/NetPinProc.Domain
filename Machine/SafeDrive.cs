#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace NetPinProc.Domain
{
    /// <summary>
    /// Queued thread-safe coil drive entry
    /// </summary>
    public class SafeCoilDrive
    {
        public string coil_name = "";
        public bool pulse = false;
        public ushort pulse_time = 30;
        public bool disable = false;
    }
}
