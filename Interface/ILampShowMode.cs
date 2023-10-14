using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// A mode for lamp shows
    /// </summary>
    public interface ILampShowMode : IMode
    {
        /// <summary>
        /// Load a new lamp show from file
        /// </summary>
        void Load(string filename, bool repeat = false, Delegate callback = null);
        /// <summary>
        /// Restart the lamp show
        /// </summary>
        void Restart();
    }
}