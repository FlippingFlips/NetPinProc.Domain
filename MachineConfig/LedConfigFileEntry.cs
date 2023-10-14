namespace NetPinProc.Domain.MachineConfig
{
    /// <summary>
    /// Represents an LED's configuration in memory
    /// </summary>
    public class LedConfigFileEntry  : ConfigFileEntryBase
    {
        ///<inheritdoc/>
        public string Name { get; set; }
        ///<inheritdoc/>
        public string Number { get; set; }
        ///<inheritdoc/>
        public string Bus { get; set; }
        /// <summary>
        /// Reverse Polarity?
        /// </summary>
        public bool Polarity { get; set; } = true;
        ///<inheritdoc/>
        public string Tags { get; set; }
    }
}
