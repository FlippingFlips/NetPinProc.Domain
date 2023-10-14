namespace NetPinProc.Domain.MachineConfig
{
    /// <summary> Machine config for Lpd8806 serial LED on PDLEd boards</summary>
    public class Lpd8806ConfigFileEntry : ConfigFileEntryBase
    {
        /// <summary> Reference name </summary>
        public string Name { get; set; }

        /// <summary> Board Id </summary>
        public byte BoardId { get; set; }

        /// <summary>
        /// 0-2 index
        /// </summary>
        public int Index { get; set; } = 0;

        /// <inheritdoc/>
        public uint First { get; set; } = 0;

        /// <inheritdoc/>
        public uint Last { get; set; } = 0;

        /// <summary> Flag to enable before registering to proc</summary>
        public bool IsEnabled { get; set; } = true;
    }
}
