namespace NetPinProc.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDisplayController
    {
        /// <summary>
        /// Sets message on display for a given time
        /// </summary>
        /// <param name="message"></param>
        /// <param name="seconds"></param>
        void SetMessage(string message, int seconds);

        /// <summary>
        /// Iterate over <see cref="Domain.IGameController.Modes"/> from lowest to highest and composites a DMD image for this
        /// point in time by checking for a layer attribute on each Mode class.
        /// <para/>
        /// If the mode has a layer attribute, that layer's CompositeNext method is called to apply that layer's
        /// next Frame to the Frame in progress.
        /// </summary>
        void Update();
    }
}