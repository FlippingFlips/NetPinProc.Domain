namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// Script for led color changes
    /// </summary>
    public class LEDScript
    {
        /// <summary>
        /// Should be in format 0xFF not 255
        /// </summary>
        public uint[] Colour { get; set; }
        /// <summary>
        /// Duration in milliseconds
        /// </summary>
        public uint Duration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public uint FadeTime { get; set; }
    }
}
