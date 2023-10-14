namespace NetPinProc.Domain
{
    /// <summary>
    /// Used for mode delays to call a method when completed
    /// </summary>
    public delegate void AnonDelayedHandler();
    /// <summary>
    /// Used for mode delays to call a method when completed
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>

    public delegate bool DelayedHandler(object param);
    /// <summary>
    /// Handler passing a switch to the method
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>

    public delegate bool SwitchAcceptedHandler(Switch sw);
}
