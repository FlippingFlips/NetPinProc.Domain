namespace NetPinProc.Domain
{
    /// <summary>
    /// Switch handler types
    /// </summary>
    public enum SwitchHandleType
    {
        /// <summary>
        /// <see cref="PinProc.EventType.SwitchOpenDebounced"/>
        /// </summary>
        open,
        /// <summary>
        /// <see cref="PinProc.EventType.SwitchOpenDebounced"/>
        /// </summary>
        active,
        /// <summary>
        /// <see cref="PinProc.EventType.SwitchClosedDebounced"/>
        /// </summary>
        inactive,
        /// <summary>
        /// <see cref="PinProc.EventType.SwitchClosedDebounced"/>
        /// </summary>
        closed
    }
}
