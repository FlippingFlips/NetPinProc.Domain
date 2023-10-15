using NetPinProc.Domain.Pdb;
using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// PDB or direct Coil
    /// </summary>
    public class Coil
    {
        /// <summary>
        /// 
        /// </summary>
        public byte BankNum;

        /// <summary>
        /// 
        /// </summary>
        public byte BoardNum;

        /// <summary>
        /// Dedicated or pdb
        /// </summary>
        public string CoilType = "";

        /// <summary>
        /// 
        /// </summary>
        public byte OutputNum;

        private PDBConfig pdb;
        /// <summary>
        /// Checks whether coil is <see cref="IsDirectCoil(string)"/> or <see cref="IsPdbCoil(string)"/>
        /// </summary>
        /// <param name="pdb"></param>
        /// <param name="number_str"></param>
        public Coil(PDBConfig pdb, string number_str)
        {
            this.pdb = pdb;
            string upper_str = number_str.ToUpper();

            if (this.IsDirectCoil(upper_str))
            {
                this.CoilType = "dedicated";
                this.BankNum = (byte)((Int32.Parse(number_str.Substring(1)) - 1) / 8);
                this.OutputNum = (byte)(Int32.Parse(number_str.Substring(1)));
            }
            else if (this.IsPdbCoil(number_str))
            {
                this.CoilType = "pdb";
                PDBAddress addr = PDBFunctions.DecodePdbAddress(number_str, this.pdb.aliases.ToArray());
                this.BoardNum = addr.Board;
                this.BankNum = addr.Bank;
                this.OutputNum = addr.Output;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="BankNum"/> if dedicated</returns>
        public int Bank()
        {
            if (this.CoilType == "dedicated")
                return this.BankNum;
            else if (this.CoilType == "pdb")
                return this.BoardNum * 2 + this.BankNum;
            else
                return -1;
        }

        /// <summary>
        /// Checks if direct coil. These start with C. eg: C01, C14
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsDirectCoil(string str)
        {
            int testNum;
            if (str.Length < 2 || str.Length > 3) return false;
            if (str[0] != 'C') return false;
            if (Int32.TryParse(str.Substring(1), out testNum)) return false;
            return true;
        }

        /// <summary>
        /// Checks the <see cref="PDBFunctions.IsPdbAddress(string, DriverAlias[])"/>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsPdbCoil(string str) => PDBFunctions.IsPdbAddress(str, this.pdb.aliases.ToArray());

        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="OutputNum"/></returns>
        public int Output() => this.OutputNum;
    }
}
