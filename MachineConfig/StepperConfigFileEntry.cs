namespace NetPinProc.Domain.MachineConfig
{
    /// <summary>
    /// Machine config for Steppers on PDLEd boards
    /// </summary>
    public class StepperConfigFileEntry : ConfigFileEntryBase
    {
        /// <summary> Reference name </summary>
        public string Name { get; set; }

        /// <summary> Stepper 0 or 1 </summary>
        public bool IsStepper1 { get; set; } = true;

        /// <summary> Board Id for the servo</summary>
        public byte BoardId { get; set; }

        /// <summary> Stepper speed </summary>
        public uint Speed { get; set; }

        /// <summary> Flag to enable the servo read by config before registering to proc</summary>
        public bool IsEnabled { get; set; } = true;
    }
}
