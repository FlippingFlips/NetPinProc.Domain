using NetPinProc.Domain.PinProc;
using System.Collections.Generic;

namespace NetPinProc.Domain
{
    /// <summary>
    /// A mode queue for <see cref="IMode"/>
    /// </summary>
    public interface IModeQueue
    {
        /// <summary>
        /// Mode list
        /// </summary>
        List<IMode> Modes { get; }
        /// <summary>
        /// Add a mode to the list
        /// </summary>
        /// <param name="mode"></param>
        void Add(IMode mode);
        /// <summary>
        /// Clear all modes
        /// </summary>
        void Clear();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        void HandleEvent(Event evt);
        /// <summary>
        /// Remove a mode from queue
        /// </summary>
        /// <param name="mode"></param>
        void Remove(IMode mode);
        /// <summary>
        /// On Tick from game controller
        /// </summary>
        void Tick();
        /// <summary>
        /// Get a print list of running modes
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}