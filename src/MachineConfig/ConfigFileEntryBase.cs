namespace NetPinProc.Domain.MachineConfig
{
    /// <summary>
    /// Base class for extra fields to display in a UI, wire colours, display name
    /// </summary>
    public abstract class ConfigFileEntryBase
    {
        /// <summary> Extra for UI </summary>
        public string DisplayName { get; set; }

        /// <summary> Connection fitting </summary>
        public string Conn { get; set; }

        /// <summary> Location being Playfield, Cabinet etc </summary>
        public string Location { get; set; }

        /// <summary> Position on table. Could be used for milimetre then display on overlay UI or export</summary>
        public double? XPos { get; set; }

        /// <summary> Position on table. Could be used for milimetre then display on overlay UI or export</summary>
        public double? YPos { get; set; }

        /// <summary> Type. For a switch this could be Opto, Rollover, Leaf etc. <para/>
        /// For coils this type could be the coil part number
        /// </summary>
        public string ItemType { get; set; }
    }
}
