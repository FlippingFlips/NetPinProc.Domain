namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// PDBSwitch
    /// </summary>
    public class PDBSwitch
    {
        private int sw_number;

        /// <summary>
        /// Initialize switch. Creates switch number depending on number_str given/>
        /// </summary>
        /// <param name="number_str">Can be dedicated 'SD' or contain '/' for matrix</param>
        public PDBSwitch(string number_str)
        {
            var upperStr = number_str.ToUpper();
            if (upperStr.StartsWith("SD"))
            {
                this.SwitchType = PdbSwitchType.dedicated;
                sw_number = int.Parse(upperStr.Substring(2));
            }
            else if (upperStr.Contains("/"))
            {
                this.SwitchType = PdbSwitchType.matrix;
                sw_number = ParseMatrixNum(upperStr);
            }
            else
            {
                this.SwitchType = PdbSwitchType.proc;
                sw_number = int.Parse(number_str);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PdbSwitchType SwitchType { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="sw_number"/></returns>
        public int ProcNum() => sw_number;
        /// <summary>
        /// Parse matrix eg: 0/1 
        /// </summary>
        /// <param name="upperStr"></param>
        /// <returns></returns>
        private int ParseMatrixNum(string upperStr)
        {
            var crList = upperStr.Split('/');
            return (32 + int.Parse(crList[0]) * 16 + int.Parse(crList[1]));
        }
    }
}
