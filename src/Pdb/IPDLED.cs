using System.Numerics;

namespace NetPinProc.Domain.Pdb
{
    /// <summary>
    /// PDLED board that writes data to the PROC through the IProcDevice. <para/>
    /// Used for LEDS to change color, fade colors and time
    /// </summary>
    public interface IPDLED
    {        
        /// <summary>
        /// Board address. Used when writing the initial register
        /// </summary>
        uint BoardAddress { get; }

        /// <summary> Control register settings for serial, steppers</summary>
        ControlRegister ControlRegister { get; }

        /// <summary> Register for up to 12 servos </summary>
        ServoRegister ServoRegister { get; }

        /// <summary>
        /// Writes the address of the board to the first register bus
        /// </summary>
        /// <param name="addr"></param>
        void WriteAddress(uint addr);

        /// <summary>
        /// Writes address then the color to an led through the proc on BUS 1
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        void WriteColor(uint index, uint color);

        /// <summary>
        /// Writes to the configuration register.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        void WriteConfigRegister(uint addr, uint data);

        /// <summary>
        /// Writes the <see cref="ControlRegister"/> on this board to the proc. <para/>
        /// Uses WriteSerialControl to send data address 0 which is the control register
        /// </summary>
        void WriteControlRegister();

        /// <summary>
        /// Writes the <see cref="ServoRegister"/> on this board to the proc. <para/>
        /// </summary>
        /// <param name="maxServoValue"></param>
        void WriteServoRegister(uint maxServoValue);

        /// <summary>
        /// Writes address then the fade destination color to an led through the proc on BUS 2
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        void WriteFadeColor(uint index, uint color);

        /// <summary>
        /// Change fade time of leds on the board. <para/>
        /// Fade rates have 4ms resolutions. 1= 4msm 2 = 8ms...256 = 1s
        /// </summary>
        /// <param name="time"></param>
        void WriteFadeTime(uint time);

        /// <summary>
        /// Writes settings to this board for Ws2811 serial leds <para/>
        /// MPF: github.com/missionpinball/mpf/blob/dev/mpf/platforms/p_roc_common.py
        /// </summary>
        /// <param name="lbt">WS281x Low Bit Time – The total number of 32 MHz clock cycles to drive the data signal high for a low bit. (Default is 13 for WS2812)</param>
        /// <param name="hbt">WS281x High Bit Time – The total number of 32 MHz clock cycles to drive the data signal high for a high bit. (Default is 24 for WS2812)</param>
        /// <param name="ebt">WS281x End Bit Time – The total number 32 MHz clock cycles to drive the data signal high and then low for a single data bit. (Default is 40 for WS2812)</param>
        /// <param name="rbt">WS281x Reset Bit Time – The total number of 32 MHz clock cycles to drive the data bit low to complete the update cycle for the entire chain. (Default is 1603 for WS2812)</param>
        void WriteWS281xControlRegister(uint lbt = 13, uint hbt = 24, uint ebt = 40, uint rbt = 1603);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        void WriteWS281xRangeRegister(uint index, uint first, uint last);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        void WriteLpd8806RangeRegister(uint index, uint first, uint last);
        
    }
}
