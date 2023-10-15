using NetPinProc.Domain.Pdb;
using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// PDB Lamp
    /// </summary>
    public class Lamp
    {
        /// <summary>
        /// dedicated, pdb or unknown
        /// </summary>
        public string LampType;

        private byte BankNum = 0;
        private byte Output;
        private PDBConfig pdb;
        private byte sink_banknum;
        private byte sink_boardnum;
        private byte sink_outputnum;
        private byte source_banknum;
        private byte source_boardnum;
        private byte source_outputnum;

        /// <summary>
        /// Sets up a dedicated or pdb lamp. If pdb then the boards are setup
        /// </summary>
        /// <param name="pdb"></param>
        /// <param name="number_str"></param>
        public Lamp(PDBConfig pdb, string number_str)
        {
            this.pdb = pdb;
            string upper_str = number_str.ToUpper();
            if (IsDirectLamp(upper_str))
            {
                this.LampType = "dedicated";
                this.Output = (byte)(Int32.Parse(number_str.Substring(1)));
            }
            else if (IsPDBLamp(number_str))
            {
                this.LampType = "pdb";
                string[] addr_parts = PDBFunctions.SplitMatrixAddressParts(number_str);
                string source_addr, sink_addr;
                source_addr = addr_parts[0];
                sink_addr = addr_parts[1];

                PDBAddress addr = PDBFunctions.DecodePdbAddress(source_addr, pdb.aliases.ToArray());
                source_boardnum = addr.Board;
                source_banknum = addr.Bank;
                source_outputnum = addr.Output;

                addr = PDBFunctions.DecodePdbAddress(sink_addr, pdb.aliases.ToArray());
                sink_boardnum = addr.Board;
                sink_banknum = addr.Bank;
                sink_outputnum = addr.Output;
            }
            else
            {
                LampType = "unknown";
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="BankNum"/></returns>
        public byte DedicatedBank() => this.BankNum;
        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="Output"/></returns>
        public byte DedicatedOutput() => this.Output;

        /// <summary>
        /// Checks if dedicated. Prefixed with L. L16, L84
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsDirectLamp(string str)
        {
            int testNum;
            if (str.Length < 2 || str.Length > 3) return false;
            if (str[0] != 'L') return false;
            if (!Int32.TryParse(str.Substring(1), out testNum)) return false;
            return true;
        }

        /// <summary>
        /// Checks the address string to check if PDBLamp
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsPDBLamp(string str)
        {
            string[] _params = PDBFunctions.SplitMatrixAddressParts(str);
            if (_params.Length != 2) return false;
            foreach (string addr in _params)
            {
                if (!PDBFunctions.IsPdbAddress(addr, this.pdb.aliases.ToArray()))
                {
                    Console.WriteLine("Not PDB address! " + addr);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>sink bank from <see cref="sink_boardnum"/> and <see cref="sink_banknum"/></returns>
        public byte SinkBank() => (byte)(sink_boardnum * 2 + sink_banknum);
        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="sink_boardnum"/></returns>
        public byte SinkBoard() => this.sink_boardnum;
        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="sink_outputnum"/></returns>
        public byte SinkOutput() => sink_outputnum;
        /// <summary>
        /// 
        /// </summary>
        /// <returns>source bank from <see cref="source_boardnum"/> and <see cref="source_banknum"/></returns>
        public byte SourceBank() => (byte)(source_boardnum * 2 + source_banknum);
        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="source_boardnum"/></returns>
        public byte SourceBoard() => this.source_boardnum;
        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="source_outputnum"/></returns>
        public byte SourceOutput() => source_outputnum;
    }
}
