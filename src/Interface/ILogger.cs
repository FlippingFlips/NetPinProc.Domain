using NetPinProc.Domain.PinProc;

namespace NetPinProc.Domain
{
    ///<inheritdoc/>
    public interface ILogger
    {
        /// <summary>
        /// Logging level
        /// </summary>
        LogLevel LogLevel { get; set; }

        /// <summary>
        /// Log with no levels
        /// </summary>
        void Log(string text);

        ///<inheritdoc/>
        void Log(string text, LogLevel logLevel = LogLevel.Info);

        /// <summary>
        /// Log object array for level
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logObjs"></param>
        void Log(LogLevel logLevel = LogLevel.Info, params object[] logObjs);

        /// <summary>
        /// Log object array
        /// </summary>
        /// <param name="logObjs"></param>
        void Log(params object[] logObjs);
    }
}
