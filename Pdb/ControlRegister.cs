namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// PD-LED control register. Register model for extra devices like serial leds, stepper motors. <para/>
    /// </summary>
    public class ControlRegister
    {
        /// <summary>Serial LED chain control - LED83</summary>
        public byte UseWS281x0 { get; set; } = 1;

        /// <summary>Serial LED chain control - LED82</summary>
        public byte UseWS281x1 { get; set; }

        /// <summary>Serial LED chain control - LED81</summary>
        public byte UseWS281x2 { get; set; }

        /// <summary>LPD8806 chains - LED81</summary>
        public byte UseLpd880x0 { get; set; }

        /// <summary>LPD8806 chains - LED 78, 79, 90</summary>
        public byte UseLpd880x1 { get; set; }

        /// <summary>LPD8806 chains - LED 75,76,77</summary>
        public byte UseLpd880x2 { get; set; }

        /// <summary>Stepper 0 - J8 12,13,14 LED 78, 79, 90</summary>
        public byte UseStepper0 { get; set; }

        /// <summary>Stepper 1 - J8 7,8,9 or LED 75,76,77</summary>
        public byte UseStepper1 { get; set; }

        /// <summary>
        /// Converts the model properites into data bit address for writing to PROC
        /// </summary>
        /// <returns></returns>
        public uint GetDataBits()
        {
            return (uint)
                ((UseWS281x0 * 1 << 0) +
                (UseWS281x1 * 1 << 1) +
                (UseWS281x2 * 1 << 2) +
                (UseLpd880x0 * 1 << 3) +
                (UseLpd880x1 * 1 << 4) +
                (UseLpd880x2 * 1 << 5) +
                (UseStepper0 * 1 << 8) +
                (UseStepper1 * 1 << 9));
        }
    }
}
