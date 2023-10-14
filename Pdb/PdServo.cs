namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// PD-LED board servo
    /// </summary>
    public class PdServo
    {
        private IPDLED board;

        private readonly IProcDevice Proc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="name"></param>
        /// <param name="boardId"></param>
        /// <param name="servoIndex"></param>
        /// <param name="minvalue">default pos</param>
        public PdServo(IProcDevice proc, string name, uint boardId, uint servoIndex, int minvalue)
        {
            Proc = proc;
            Name = name;
            BoardId = boardId;
            ServoIndex = servoIndex;
            Minvalue = minvalue;

            //get the board this led address uses
            var pdLedBoard = PdLeds.GetPdLedBoard(boardId);
            //assign the board to the led if not create one under this address
            if (pdLedBoard != null) { board = pdLedBoard; }
            else
            {
                this.board = new PDLED(proc, boardId);
                PdLeds.PDLEDS.Add(this.board);
            }
        }

        /// <summary>
        /// Moves servo position. MPF.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(int position)
        {            
            var value = (position * (255 - Minvalue)) + Minvalue;
            Proc.Logger.Log($"set servo pos: {Position} to {value}", PinProc.LogLevel.Debug);

            Position = value;

            board.WriteColor(72 + ServoIndex, (uint)value);
        }

        /// <inheritdoc/>
        public void Stop() => board.WriteColor(72 + ServoIndex, 0);

        /// <inheritdoc/>
        public string Name { get; }
        /// <inheritdoc/>
        public uint BoardId { get; }
        /// <inheritdoc/>
        public uint ServoIndex { get; }
        /// <inheritdoc/>
        public int Minvalue { get; }
        /// <inheritdoc/>
        public int Position { get; private set; }
    }
}
