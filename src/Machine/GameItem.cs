namespace NetPinProc.Domain
{
    /// <summary>
    /// Base class for 'Driver' and 'Switch'
    /// </summary>
    public class GameItem : IGameItem
    {
        /// <summary>
        /// P-ROC device
        /// </summary>
        protected readonly IProcDevice proc;

        /// <summary>
        /// Name of this item
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// Integer value for this item that provides a mapping to the hardware
        /// </summary>
        protected ushort _number = 0;

        /// <summary>
        /// LED string number
        /// </summary>

        protected string _strNumber;

        /// <summary>
        /// Initializes parameters
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <param name="strNumber"></param>
        public GameItem(IProcDevice proc, string name, ushort number, string strNumber = "")
        {
            this.proc = proc;
            this._name = name;
            this._number = number;
            _strNumber = strNumber;
        }

        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Number of item
        /// </summary>
        public ushort Number
        {
            get { return _number; }
            set { _number = value; }
        }
    }
}
