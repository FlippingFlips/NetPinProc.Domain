using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NetPinProc.Domain.PinProc;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Represents a game mode
    /// </summary>
    public partial class Mode : IMode
    {
        /// <summary>
        /// constant return values for switch handling
        /// </summary>
        public const bool SWITCH_CONTINUE = false;
        /// <summary>
        /// constant return values for switch handling
        /// </summary>
        public const bool SWITCH_STOP = true;

        /// <inheritdoc/>
        public ILayer Layer { get; set; }

        /// <summary>
        /// List of delayed switch handlers
        /// TODO: Consider renaming this to something that makes more sense
        /// </summary>
        private List<AcceptedSwitch> _accepted_switches = new List<AcceptedSwitch>();

        /// <summary>
        /// A list of delayed events/callbacks
        /// </summary>
        private List<Delayed> _delayed = new List<Delayed>();

        /// <summary>
        /// Creates a new Mode. Runs <see cref="ScanSwitchHandlers"/>
        /// </summary>
        /// <param name="game">The parent GameController object</param>
        /// <param name="priority">The priority of this mode in the queue</param>
        public Mode(IGameController game, int priority)
        {
            this.Game = game;
            this.Priority = priority;
            ScanSwitchHandlers();
        }

        /// <inheritdoc/>
        public IGameController Game { get; set; }
        /// <inheritdoc/>        
        public int Priority { get; set; }
        /// <inheritdoc/>
        public void AddSwitchHandler(string Name, SwitchHandleType handleType = SwitchHandleType.active, double Delay = 0, SwitchAcceptedHandler Handler = null)
        {
            EventType adjusted_event_type;
            Switch sw = Game.Switches[Name];

            switch (handleType)
            {                
                case SwitchHandleType.active:
                    if(sw.Type == SwitchType.NO) adjusted_event_type = EventType.SwitchClosedDebounced;
                    else adjusted_event_type = EventType.SwitchOpenDebounced;
                    break;
                case SwitchHandleType.inactive:
                    if (sw.Type == SwitchType.NO) adjusted_event_type = EventType.SwitchOpenDebounced;
                    else adjusted_event_type = EventType.SwitchClosedDebounced;
                    break;
                case SwitchHandleType.closed:
                    adjusted_event_type = EventType.SwitchClosedDebounced;
                    break;
                case SwitchHandleType.open:
                default:
                    adjusted_event_type = EventType.SwitchOpenDebounced;
                    break;
            }
            
            AcceptedSwitch asw = new AcceptedSwitch(Name, adjusted_event_type, Delay, Handler, sw);
            if (!_accepted_switches.Contains(asw))
                _accepted_switches.Add(asw);
        }
        /// <inheritdoc/>
        public void CancelDelayed(string Name) => _delayed = _delayed.Where<Delayed>(x => x.Name != Name).ToList<Delayed>();
        /// <summary>
        /// Compares the priority. never used...
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is Mode)
                return ((Mode)obj).Priority.CompareTo(Priority);
            else
                return -1;
        }
        /// <inheritdoc/>
        public void Delay(string Name, EventType Event_Type, double Delay, Delegate Handler, object Param = null)
        {
            Game.Logger.Log(String.Format("Adding delay name={0} Event_Type={1} delay={2}", Name, Event_Type, Delay), LogLevel.Verbose);
            Delayed d = new Delayed(Name, Time.GetTime() + Delay, Handler, Event_Type, Param);
            _delayed.Add(d);
            _delayed.Sort();
        }
        /// <inheritdoc/>
        public void DispatchDelayed()
        {
            double t = Time.GetTime();
            int cnt = _delayed.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (_delayed[i].Time <= t)
                {
                    Game.Logger?.Log("dispatch_delayed() " + _delayed[i].Name + " " + _delayed[i].Time.ToString() + " <= " + t.ToString(), LogLevel.Verbose);
                    if (_delayed[i].Param != null)
                        _delayed[i].Handler.DynamicInvoke(_delayed[i].Param);
                    else
                        _delayed[i].Handler.DynamicInvoke(null);
                }
            }
            _delayed = _delayed.Where<Delayed>(x => x.Time > t).ToList<Delayed>();
        }
        /// <inheritdoc/>
        public bool HandleEvent(Event evt)
        {
            string sw_name = Game.Switches[(ushort)evt.Value].Name;
            bool handled = false;
            // Filter out all of the delayed events that have been disqualified by this state change.
            // Remove all items that are for this switch (sw_name) but for a different state (type).
            // In other words, keep delayed items pertaining to other switches, plus delayed items pertaining
            // to this switch for another state

            //var newDelayed = from d in _delayed
            //                 where d.Name != sw_name && (int)d.Event_Type != (int)evt.Type
            //                 select d;

            //_delayed = newDelayed.ToList<Delayed>();
            _delayed = _delayed.FindAll(d => d.Name != sw_name);

            //_delayed = _delayed.Where<Delayed>(x => sw_name == x.Name && x.Event_Type != evt.Type).ToList<Delayed>();

            foreach (AcceptedSwitch asw in
                _accepted_switches.Where<AcceptedSwitch>(
                    accepted =>
                        accepted.Event_Type == evt.Type
                        && accepted.Name == sw_name).ToList<AcceptedSwitch>())
            {
                if (asw.Delay == 0)
                {
                    bool result = asw.Handler(Game.Switches[asw.Name]);

                    if (result == SWITCH_STOP)
                        handled = true;
                }
                else
                {
                    Delay(sw_name, asw.Event_Type, asw.Delay, asw.Handler, asw.Param);
                }
            }

            return handled;
        }
        /// <inheritdoc/>
        public virtual void ModeStarted() { }
        /// <inheritdoc/>
        public virtual void ModeStopped() { }
        /// <inheritdoc/>
        public virtual void ModeTick() { }
        /// <inheritdoc/>
        public virtual void ModeTopMost() { }
        /// <inheritdoc/>
        public override string ToString() => $"{this.GetType().Name} pri={Priority}";
        /// <inheritdoc/>
        public virtual void UpdateLamps() { }

        /// <summary>
        /// Scan all statically defined switch handlers in mode classes and wire up handling events. <para/>
        /// open|closed|active|inactive
        /// </summary>
        private void ScanSwitchHandlers()
        {
            // Get all methods in the mode class that match a certain regular expression
            Type t = this.GetType();
            MethodInfo[] methods = t.GetMethods();
            string regexPattern = "sw_(?<name>[a-zA-Z0-9]+)_(?<state>open|closed|active|inactive)(?<after>_for_(?<time>[0-9]+)(?<units>ms|s))?";
            Regex pattern = new Regex(regexPattern);
            foreach (MethodInfo m in methods)
            {
                MatchCollection matches = pattern.Matches(m.Name);
                string switchName = "";
                string switchState = "";
                bool hasTimeSpec = false;
                double switchTime = 0;
                string switchUnits = "";
                foreach (Match match in matches)
                {
                    int i = 0;
                    foreach (Group group in match.Groups)
                    {
                        if (group.Success == true)
                        {
                            string gName = pattern.GroupNameFromNumber(i);
                            string gValue = group.Value;
                            if (gName == "name")
                            {
                                switchName = gValue;
                            }
                            if (gName == "state")
                                switchState = gValue;

                            if (gName == "after")
                                hasTimeSpec = true;

                            if (gName == "time")
                                switchTime = Int32.Parse(gValue);

                            if (gName == "units")
                                switchUnits = gValue;

                        }
                        i++;
                    }
                }
                if (switchName != "" && switchState != "")
                {
                    if (hasTimeSpec && switchUnits == "ms")
                        switchTime = switchTime / 1000.0;



                    SwitchAcceptedHandler swh = (SwitchAcceptedHandler)Delegate.CreateDelegate(typeof(SwitchAcceptedHandler), this, m);
                    Enum.TryParse(switchState, out SwitchHandleType SwitchHandleType);
                    AddSwitchHandler(switchName, SwitchHandleType, switchTime, swh);
                }
            }
        }
    }
}
