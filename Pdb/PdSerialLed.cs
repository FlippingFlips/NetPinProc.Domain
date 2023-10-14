using System;

namespace NetPinProc.Domain.Pdb
{
    /// <summary> Serial LEDs on PD-LED boards <para/>
    /// There are six Serial Chain Controllers, each of which connects directly to FPGA pins (1 pin for WS281x, 2 pins for LPD880x). <para/>
    /// Each Controllers is configured with the first and last LED numbers it should drive its pins. <para/>
    /// The data for all of the LEDs between the first and last LED numbers, inclusive, will be driven onto the pins during each cycle. <para/>
    /// The cycle update frequency depends on the configuration of the Serial Chain Manager.
    /// As a timing example, assume that 500 RGB LEDs are driven onto LED chains, and the chains use the
    /// default WS2812 timing.The total time it takes to update the entire chain is 
    /// ((500 RGBs* 3 (for 1500 individual LEDs) * 40 clock cycles) + 1603) * (1 / 32 MHz) = 1.925ms.
    /// </summary>
    public class PdSerialLed
    {
        private IPDLED board;

        private readonly IProcDevice Proc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="name"></param>
        /// <param name="pdSerialLedType"></param>
        /// <param name="boardId"></param>
        /// <param name="index"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        public PdSerialLed(IProcDevice proc, string name, 
            PdSerialLedType pdSerialLedType,
            uint boardId, uint index, uint first, uint last)
        {
            Proc = proc;
            Name = name;
            PdSerialLedType = pdSerialLedType;
            BoardId = boardId;
            Index = index;
            First = first;
            Last = last;

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

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public PdSerialLedType PdSerialLedType { get; }

        /// <inheritdoc/>
        public uint BoardId { get; }

        /// <inheritdoc/>
        public uint Index { get; }

        /// <inheritdoc/>
        public uint First { get; }

        /// <inheritdoc/>
        public uint Last { get; }

        /// <inheritdoc/>
        public void WriteColor(uint color)
        {
            //var ledIndex = 83 - Index;

            board.WriteColor(83, color);
            board.WriteColor(82, color);
            board.WriteColor(81, color);
        }
    }
}
