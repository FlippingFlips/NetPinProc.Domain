using System;
using NetPinProc.Domain.PinProc;

namespace NetPinProc.Domain
{
    /// <inheritdoc/>
    internal class AcceptedSwitch : IEquatable<AcceptedSwitch>
    {
        public AcceptedSwitch(string Name, EventType Event_Type, double Delay, SwitchAcceptedHandler Handler = null, object Param = null)
        {
            this.Name = Name;
            this.Event_Type = Event_Type;
            this.Delay = Delay;
            this.Handler = Handler;
            this.Param = Param;
        }

        public double Delay { get; set; }
        public EventType Event_Type { get; set; }
        public SwitchAcceptedHandler Handler { get; set; }
        public string Name { get; set; }
        public object Param { get; set; }
        public bool Equals(AcceptedSwitch other)
        {
            if (other.Delay == this.Delay && other.Name == this.Name
                && other.Event_Type == this.Event_Type && other.Handler == this.Handler)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return String.Format("<name={0} event_type={1} delay={2}>", this.Name, this.Event_Type, this.Delay);
        }
    }
}
