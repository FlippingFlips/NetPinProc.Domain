namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// PD-LED servo register.
    /// </summary>
    public class ServoRegister
    {
        /// <summary>Use Servo</summary>
        public byte UseServo0 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo1 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo2 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo3 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo4 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo5 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo6 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo7 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo8 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo9 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo10 { get; set; }
        /// <summary>Use Servo</summary>
        public byte UseServo11 { get; set; }

        /// <summary>
        /// Converts the model properites into data bit address for writing to PROC
        /// </summary>
        /// <returns></returns>
        public uint GetDataBits()
        {
            return (uint)
                (
                    (UseServo0 * 1 << 0) +
                    (UseServo1 * 1 << 1) +
                    (UseServo2 * 1 << 2) +
                    (UseServo3 * 1 << 3) +
                    (UseServo4 * 1 << 4) +
                    (UseServo5 * 1 << 5) +
                    (UseServo6 * 1 << 6) +
                    (UseServo7 * 1 << 7) +
                    (UseServo6 * 1 << 8) +
                    (UseServo6 * 1 << 9) +
                    (UseServo6 * 1 << 10) +
                    (UseServo6 * 1 << 11)
                );
        }
    }
}
