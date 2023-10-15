namespace NetPinProc.Domain.MachineConfig
{
    /// <summary>
    /// Represents a lamp's configuration in memory
    /// </summary>
    public class LampConfigFileEntry : ConfigFileEntryBase
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
