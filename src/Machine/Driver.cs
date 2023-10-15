using NetPinProc.Domain.PinProc;
using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Represents a driver in a pinball machine, such as a lamp, coil/solenoid, or flasher.
    /// </summary>
    public class Driver : GameItem, IDriver
    {
        /// <summary>
        /// Default number of milliseconds to pulse this driver.
        /// </summary>
        protected byte _default_pulse_time = (byte)30;

        /// <summary>
        /// Game Item Driver
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        public Driver(IProcDevice proc, string name, ushort number)
            : base(proc, name, number) { }

        /// <inheritdoc/>
        public double LastTimeChanged { get; set; }
        /// <inheritdoc/>
        public DriverState State
        {
            get
            {
                return this.proc.DriverGetState(this._number);
            }
            set
            {
            }
        }

        /// <summary>
        /// A default set time to pulse the coil
        /// </summary>
        public byte PulseTime
        {
            get
            {
                return _default_pulse_time;
            }
            set
            {
                _default_pulse_time = value;
            }
        }

        /// <inheritdoc/>
        public void Disable()
        {
            this.proc.DriverDisable(_number);
            this.LastTimeChanged = Time.GetTime();
        }
        /// <inheritdoc/>
        public void Enable()
        {
            this.Schedule(0xffffffff, 0, true);
        }
        /// <inheritdoc/>
        public void FuturePulse(int milliseconds = -1, UInt16 futureTime = 100)
        {
            if (milliseconds < 0) milliseconds = _default_pulse_time;
            milliseconds = milliseconds > 255 ? 255 : milliseconds;
            this.proc.DriverFuturePulse(_number, (byte)milliseconds, futureTime);
            this.LastTimeChanged = Time.GetTime();
        }
        /// <inheritdoc/>
        public void Patter(byte on_time = 10, byte off_time = 10, byte orig_on_time = 0)
        {

            this.proc.DriverPatter(_number, on_time, off_time, orig_on_time);
            this.LastTimeChanged = Time.GetTime();
        }
        /// <inheritdoc/>
        public virtual void Pulse(int milliseconds = -1)
        {
            milliseconds = milliseconds > 255 ? 255 : milliseconds;
            this.proc.DriverPulse(_number, (byte)milliseconds);
            this.LastTimeChanged = Time.GetTime();
        }
        /// <inheritdoc/>
        public void PulsedPatter(ushort on_time = 10, ushort off_time = 10, ushort run_time = 0)
        {
            if (off_time < 0 || off_time > 127)
                throw new ArgumentOutOfRangeException("off_time must be in range 0-127");
            if (on_time < 0 || on_time > 127)
                throw new ArgumentOutOfRangeException("on_time must be in range 0-127");
            if (run_time < 0 || run_time > 255)
                throw new ArgumentOutOfRangeException("run_time must be in range 0-255");

            this.proc.DriverPulsedPatter(_number, off_time, on_time, run_time);
            this.LastTimeChanged = Time.GetTime();
        }
        /// <inheritdoc/>
        public void ReConfigure(bool polarity)
        {
            DriverState state = this.State;
            state.Polarity = polarity;
            this.proc.DriverUpdateState(ref state);
        }
        /// <inheritdoc/>
        public void Schedule(uint schedule, int cycle_seconds = 0, bool now = true)
        {
            this.proc.DriverSchedule(_number, schedule, (byte)cycle_seconds, now);
            this.LastTimeChanged = Time.GetTime();
        }
        /// <inheritdoc/>
        public virtual void Tick() { }
        /// <inheritdoc/>
        public override string ToString() => String.Format("<Driver name={0} number={1}>", this.Name, this.Number);
    }
}
