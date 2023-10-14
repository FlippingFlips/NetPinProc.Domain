using System.Collections.Generic;

namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// PDLEd board stepper
    /// </summary>
    public class PdStepper
    {
        /// <inheritdoc/>
        public readonly uint BoardAddress;
        private IPDLED board;

        /// <summary>
        /// Sets up a stepper and registering the speed
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="name"></param>
        /// <param name="boardId"></param>
        /// <param name="stepperIndex"></param>
        /// <param name="speed"></param>
        public PdStepper(IProcDevice proc, string name, uint boardId, byte stepperIndex, uint speed)
        {
            Name = name;
            BoardAddress = boardId;
            StepperIndex = stepperIndex;
            Speed = speed;

            //get the board this led address uses
            var pdLedBoard = PdLeds.GetPdLedBoard(BoardAddress);

            //assign the board to the led if not create one under this address
            if (pdLedBoard != null) { board = pdLedBoard; }
            else
            {
                this.board = new PDLED(proc, BoardAddress);
                PdLeds.PDLEDS.Add(this.board);
            }

            //setup control register properties for using steppers
            if (stepperIndex == 0)
                board.ControlRegister.UseStepper0 = 1;
            else
                board.ControlRegister.UseStepper1 = 1;

            //register the stepper speed
            board.WriteConfigRegister(22, Speed);
        }

        /// <inheritdoc/>
        public void Move(int pos)
        {
            var sIndex = 23 + StepperIndex;
            if (pos > 0)
                board.WriteConfigRegister((uint)sIndex, (uint)pos);
            else
            {
                board.WriteConfigRegister((uint)sIndex, (uint)(pos + 1 << 15));
            }
        }

        /// <inheritdoc/>
        public byte StepperIndex { get; }
        /// <inheritdoc/>
        public uint Speed { get; }
        /// <inheritdoc/>
        public string Name { get; set; }
    }
}
