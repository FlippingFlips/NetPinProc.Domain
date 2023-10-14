using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Time helper static methods
    /// </summary>
    public class Time
    {
        /// <summary>
        /// Get the current unix timestamp
        /// </summary>
        /// <returns>Number of seconds since the unix epoch</returns>
        public static double GetTime()
        {
            var ts = GetEpoch();
            return ts.TotalSeconds;
        }

        /// <summary>
        /// Get the current unix timestamp in milliseconds
        /// </summary>
        /// <returns>Number of seconds since the unix epoch</returns>
        public static double GetTimeMs()
        {
            var ts = GetEpoch();
            return ts.TotalMilliseconds;
        }

        static TimeSpan GetEpoch() => (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
    }
}
