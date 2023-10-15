using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Controller object that encapsulates a LampShow class and helps to restore lamp drivers to their
    /// prior state.
    /// </summary>
    public interface ILampController
    {
        /// <summary>
        /// Stops show, loads show from <see cref="ILampShowMode.Load(string, bool, Delegate)"/> and adds the game controller modes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="repeat"></param>
        /// <param name="callback"></param>
        void PlayShow(string key, bool repeat = false, Delegate callback = null);
        /// <summary>
        /// Adds a show to the Shows
        /// </summary>
        /// <param name="key"></param>
        /// <param name="show_file"></param>
        void RegisterShow(string key, string show_file);
        /// <inheritdoc/>
        void RestorePlayback();
        /// <summary>
        /// Restores the lamps from a given key
        /// </summary>
        /// <param name="key"></param>
        void RestoreState(string key);
        /// <summary>
        /// Saves state from all lamps in the game
        /// </summary>
        /// <param name="key"></param>
        void SaveState(string key);
        /// <inheritdoc/>
        void StopShow();
    }
}