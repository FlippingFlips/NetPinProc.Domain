using System.Collections.Generic;
using System.Linq;

namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// Holds a list of <see cref="IPDLED"/> boards (PD-LED). <para/>
    /// </summary>
    public static class PdLeds
    {
        /// <summary>
        /// When an LED from the machine config is added then a PD-LED board will be created and added to this list (if it doesn't already exist). <para/>
        /// Use this to acces the boards. Standard use LEDs should be done from the game.LEDS
        /// </summary>
        public static readonly List<IPDLED> PDLEDS = new List<IPDLED>();

        /// <summary> Returns a PDLED board from the list by board address </summary>
        /// <param name="boardAddress"></param>
        /// <returns>null if doesn't exist</returns>
        public static IPDLED GetPdLedBoard(uint boardAddress) =>
            PDLEDS.FirstOrDefault(x => x.BoardAddress == boardAddress);
    }
}
