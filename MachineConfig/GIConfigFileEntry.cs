namespace NetPinProc.Domain.MachineConfig
{
    /// <summary>
    /// Represents a GI string list element in memory
    /// </summary>
    public class GIConfigFileEntry : ConfigFileEntryBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tags { get; set; }
    }
}
