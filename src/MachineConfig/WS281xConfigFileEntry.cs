namespace NetPinProc.Domain.MachineConfig
{
    /// <summary> Machine config for Ws2811 serial LED on PDLEd boards</summary>
    public class WS281xConfigFileEntry : ConfigFileEntryBase
    {
        /// <summary> Reference name </summary>
        public string Name { get; set; }

        /// <summary> Board Id</summary>
        public byte BoardId { get; set; }

        /// <summary> 0-2 index </summary>
        public int Index { get; set; } = 0;

        /// <summary>First address </summary>
        public uint First { get; set; } = 0;

        /// <summary>Last address </summary>
        public uint Last { get; set; } = 0;

        /// <summary> Flag to enable before registering to proc</summary>
        public bool IsEnabled { get; set; } = true;
    }
}
