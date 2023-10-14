using NetPinProc.Domain.PinProc;
using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Represents a driver in a pinball machine, such as a lamp, coil/solenoid, or flasher. <see cref="IGameItem"/>
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Last time this driver's state was modified
        /// </summary>
        double LastTimeChanged { get; set; }
        /// <summary>
        /// Name of the game item
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Number of the game item
        /// </summary>
        ushort Number { get; set; }

        /// <summary>
        /// Pulse time of the driver
        /// </summary>
        byte PulseTime { get; set; }

        /// <summary>
        /// Current state
        /// </summary>
        DriverState State { get; set; }
        /// <summary>
        /// Disables/turns off the driver
        /// </summary>
        void Disable();
        /// <summary>
        /// Enables this driver indefinitely.
        /// <para/>
        /// WARNING!!!
        /// Never use this method with high voltage drivers such as coils and flashers!
        /// Instead, use time-limited methods such as 'pulse' and 'schedule'.
        /// </summary>
        void Enable();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="futureTime"></param>
        void FuturePulse(int milliseconds = -1, UInt16 futureTime = 100);
        /// <summary>
        /// Enables a pitter-patter sequence. <para/>
        /// It starts by activating the driver for 'orig_on_time' milliseconds. <para/>
        /// Then it repeatedly turns the driver on for 'on_time' milliseconds and off for 'off_time' milliseconds
        /// </summary>
        /// <param name="on_time"></param>
        /// <param name="off_time"></param>
        /// <param name="orig_on_time"></param>
        void Patter(byte on_time = 10, byte off_time = 10, byte orig_on_time = 0);

        /// <summary>
        /// Enables the driver for the specified number of milliseconds. <para/>
        /// If no parameters are provided, then the default pulse time is used.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to pulse the coil</param>
        void Pulse(int milliseconds = -1);
        /// <summary>
        /// Enables a pitter-patter response that runs for 'run_time' milliseconds
        /// 
        /// Until it ends, the sequence repeatedly turns the driver on for 'on_time' milliseconds and off for 'off_time' milliseconds.
        /// </summary>
        /// <param name="on_time"></param>
        /// <param name="off_time"></param>
        /// <param name="run_time"></param>
        void PulsedPatter(ushort on_time = 10, ushort off_time = 10, ushort run_time = 0);
        /// <summary>
        /// Updates p-roc device with new polarity for driver
        /// </summary>
        /// <param name="polarity"></param>
        void ReConfigure(bool polarity);
        /// <summary>
        /// Schedules this driver to be enabled according to the given 'schedule' bitmask
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="cycle_seconds"></param>
        /// <param name="now"></param>
        void Schedule(uint schedule, int cycle_seconds = 0, bool now = true);
        /// <summary>
        /// 
        /// </summary>
        void Tick();
    }
}
