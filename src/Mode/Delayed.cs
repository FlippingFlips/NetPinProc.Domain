using System;
using NetPinProc.Domain.PinProc;

namespace NetPinProc.Domain
{
    internal class Delayed : IComparable<Delayed>
    {
        public Delayed(string Name, double Time, Delegate Handler = null, EventType Event_Type = EventType.SwitchClosedDebounced, object Param = null)
        {
            this.Name = Name;
            this.Time = Time;
            this.Handler = Handler;
            this.Event_Type = Event_Type;
            this.Param = Param;
        }

        public EventType Event_Type { get; set; }
        public Delegate Handler { get; set; }
        public string Name { get; set; }
        public object Param { get; set; }
        public double Time { get; set; }
        public int CompareTo(Delayed other)
        {
            return other.Time.CompareTo(this.Time);
        }

        public override string ToString()
        {
            return String.Format("name={0} time={1} event_type={2}", this.Name, this.Time, this.Event_Type);
        }
    }
}
