using NetPinProc.Domain.PinProc;

namespace NetPinProc.Domain
{
    /// <summary>
    /// A Fake P-ROC / P3-ROC device
    /// </summary>
    public interface IFakeProcDevice : IProcDevice
    {
        /// <summary>
        /// Adds a switch to the events queue.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="event_type">Use the De-bounced, not non de-bounced</param>
        void AddSwitchEvent(ushort number, EventType event_type);
        /// <summary>
        /// 
        /// </summary>
        AttrCollection<ushort, string, IVirtualDriver> Drivers { get; }
    }
}
