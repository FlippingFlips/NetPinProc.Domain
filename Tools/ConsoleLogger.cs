using NetPinProc.Domain.PinProc;
using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Console WriteLine logger
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <inheritdoc/>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Init default level Verbose
        /// </summary>
        public ConsoleLogger() { }

        /// <summary>
        /// Init with level
        /// </summary>
        /// <param name="logLevel"></param>
        public ConsoleLogger(LogLevel logLevel = LogLevel.Verbose) => LogLevel = logLevel;

        /// <inheritdoc/>
        public void Log(string text) => Console.WriteLine(text);
        /// <inheritdoc/>
        public void Log(string text, LogLevel logLevel = LogLevel.Info)
        {
            if(logLevel <= LogLevel) Console.WriteLine(text);
        }
        /// <inheritdoc/>
        public void Log(LogLevel logLevel = LogLevel.Info, params object[] logObjs)
        {
            if (logLevel <= LogLevel)
            {
                Log(logObjs);
            }
        }
        /// <inheritdoc/>
        public void Log(params object[] logObjs) 
        {
            string format = string.Empty;
            for (int i = 0; i < logObjs.Length; i++) { format += $"{{{i}}} "; }
            Console.WriteLine(format, logObjs);
        }
    }
}
