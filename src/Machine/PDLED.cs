using NetPinProc.Domain.Pdb;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Represents a PD-LED board
    /// TODO: collection of servos. Can be 12 servos that start on LED Pin 72
    /// </summary>
    public class PDLED : IPDLED
    {
        const byte PROC_OUTPUT_MODULE = 3;
        const int PROC_PDB_BUS_ADDRESS = 0xC00;
        private readonly uint baseRegAddress;
        private readonly IProcDevice proc;
        private uint fadeTime;

        /// <summary>
        /// Create PDLED on board address. <para/>
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="boardAddress"></param>
        public PDLED(IProcDevice proc, uint boardAddress)
        {
            this.proc = proc;
            this.BoardAddress = boardAddress;
            baseRegAddress = 0x01000000 | (boardAddress & 0x3F) << 16;
            fadeTime = 0;
        }

        ///<inheritdoc/>
        public uint BoardAddress { get; private set; }

        ///<inheritdoc/>
        public ControlRegister ControlRegister { get; } = new ControlRegister();

        ///<inheritdoc/>
        public ServoRegister ServoRegister { get; } = new ServoRegister();

        ///<inheritdoc/>
        public void WriteAddress(uint addr)
        {
            uint data = baseRegAddress | (addr % 0xFF);
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);

            data = baseRegAddress | (6 << 8) | ((addr >> 8) & 0xFF);
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
        }

        ///<inheritdoc/>
        public void WriteControlRegister() =>
            WriteSerialControl(ControlRegister.GetDataBits());

        /// <summary>
        /// PDLED address register 7 (data)
        /// </summary>
        /// <param name="data"></param>
        public void WriteRegData(uint data)
        {
            uint addr = baseRegAddress | (7 << 8) | data;
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref addr);
        }

        ///<inheritdoc/>
        public void WriteColor(uint index, uint color)
        {
            WriteAddress(index);
            var data = baseRegAddress | (1 << 8) | (color & 0xFF);
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
        }

        /// <summary>
        /// Write to the boards config register
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public void WriteConfigRegister(uint addr, uint data)
        {
            WriteAddress(data);
            WriteRegData(addr);
        }

        ///<inheritdoc/>
        public void WriteWS281xControlRegister(uint lbt, uint hbt, uint ebt, uint rbt)
        {
            WriteConfigRegister(4, lbt);
            WriteConfigRegister(5, hbt);
            WriteConfigRegister(6, ebt);
            WriteConfigRegister(7, rbt);
        }

        ///<inheritdoc/>
        public void WriteFadeColor(uint index, uint color)
        {
            WriteAddress(index);
            var data = baseRegAddress | (2 << 8) | (color & 0xFF);
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
        }

        ///<inheritdoc/>
        public void WriteFadeTime(uint time)
        {
            if(time != fadeTime)
            {
                fadeTime = time;
                var data = baseRegAddress | (3 << 8) | (time & 0xFF);
                proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
                data = baseRegAddress | (4 << 8) | (time >> 0xFF) & 0xFF;
                proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
            }
        }

        /// <summary>
        /// Writes to the Control register 0 which sets up serial led, servos, steppers
        /// </summary>
        /// <param name="bitmask"></param>
        public void WriteSerialControl(uint bitmask) => WriteConfigRegister(0, bitmask);

        /// <summary>
        /// Indirect data registers can be written to the PD-LED board on address 7. <para/>
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(uint data)
        {
            var writeDate = baseRegAddress | (7 << 8) | data;
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref writeDate);
        }

        ///<inheritdoc/>
        public void WriteServoRegister(uint maxServoValue)
        {
            WriteConfigRegister(20, ServoRegister.GetDataBits());
            WriteConfigRegister(21, maxServoValue);
        }

        ///<inheritdoc/>
        public void WriteWS281xRangeRegister(uint index, uint first, uint last)
        {
            WriteConfigRegister(8 + index * 2, first);
            WriteConfigRegister(9 + index * 2, last);
        }

        ///<inheritdoc/>
        public void WriteLpd8806RangeRegister(uint index, uint first, uint last)
        {
            WriteConfigRegister(16 + index * 2, first);
            WriteConfigRegister(17 + index * 2, last);
        }
    }
}
