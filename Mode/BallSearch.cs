using NetPinProc.Domain.PinProc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetPinProc.Domain.Modes
{
    /// <summary>
    /// Pops coil when ended
    /// </summary>
    /// <param name="coil"></param>
    public delegate void BallSearchCoilDelayHandler(string coil);
    /// <summary>
    /// Performs search
    /// </summary>
    /// <param name="completion_wait_time"></param>
    /// <param name="completion_handler"></param>
    public delegate void BallSearchDelayHandler(int completion_wait_time, Delegate completion_handler);
    /// <summary>
    /// Removes the special mode
    /// </summary>
    /// <param name="special_handler_mode"></param>
    public delegate void RemoveSpecialHandlerDelegate(Mode special_handler_mode);

    /// <summary>
    /// Ball search mode
    /// </summary>
    public class BallSearch : Mode
    {
        bool _enabled;
        string[] coils;
        int countdown_time;
        string[] enable_switch_names;
        Dictionary<string, string> reset_switches;
        Mode[] special_handler_modes;
        Dictionary<string, string> stop_switches;
        /// <summary>
        /// Sets up all coils and switches for ball search <para/>
        /// Adds switch handlers to Reset and Stop
        /// </summary>
        /// <param name="game"></param>
        /// <param name="countdown_time"></param>
        /// <param name="coils"></param>
        /// <param name="reset_switches"></param>
        /// <param name="stop_switches"></param>
        /// <param name="enable_switch_names"></param>
        /// <param name="special_handler_modes"></param>
        public BallSearch(IGameController game, int countdown_time,
            string[] coils = null, Dictionary<string, string> reset_switches = null, Dictionary<string, string> stop_switches = null,
            string[] enable_switch_names = null, Mode[] special_handler_modes = null)
            : base(game, 8)
        {
            if (stop_switches == null) this.stop_switches = new Dictionary<string, string>();
            else this.stop_switches = stop_switches;
            if (coils == null) this.coils = new string[] { };
            else this.coils = coils;
            if (reset_switches == null) this.reset_switches = new Dictionary<string, string>();
            else this.reset_switches = reset_switches;
            if (special_handler_modes == null) this.special_handler_modes = new Mode[] { };
            else this.special_handler_modes = special_handler_modes;
            //TODO: Not used
            if (enable_switch_names == null) this.enable_switch_names = new string[] { };
            else this.enable_switch_names = enable_switch_names;            

            _enabled = false;
            this.countdown_time = countdown_time;

            foreach (string sw in reset_switches.Keys)
            {
                Enum.TryParse(reset_switches[sw], out SwitchHandleType handleType);
                AddSwitchHandler(sw, handleType, 0, new SwitchAcceptedHandler(Reset));
            }
            foreach (string sw in stop_switches.Keys)
            {
                Enum.TryParse(stop_switches[sw], out SwitchHandleType handleType);
                AddSwitchHandler(sw, handleType, 0, new SwitchAcceptedHandler(Stop));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disable()
        {
            Stop(null);
            _enabled = false;
        }

        /// <summary>
        /// Enables and resets
        /// </summary>
        public void Enable()
        {
            _enabled = true;
            Reset(null);
        }

        /// <summary>
        /// Runs the ball search through all coils
        /// </summary>
        /// <param name="completion_wait_time"></param>
        /// <param name="completion_handler"></param>
        public void PerformSearch(int completion_wait_time = 0, Delegate completion_handler = null)
        {
            double delay = 0.150;
            foreach (string coil in coils)
            {
                Delay(nameof(PopCoil), EventType.None, delay, new BallSearchCoilDelayHandler(PopCoil), coil);
                delay += 0.150;
            }
            Delay(nameof(StartSpecialHandlerModes), EventType.None, delay, new AnonDelayedHandler(StartSpecialHandlerModes));

            if (completion_wait_time != 0) return;
            else
            {
                CancelDelayed(nameof(PerformSearch));
                Delay(nameof(PerformSearch), EventType.None, countdown_time, new BallSearchDelayHandler(PerformSearch));
            }
        }

        /// <summary>
        /// Pulses the coil by name
        /// </summary>
        /// <param name="coil"></param>
        public void PopCoil(string coil) => Game.Coils[coil].Pulse();

        /// <summary>
        /// Removes the special mode handler from the game
        /// </summary>
        /// <param name="special_handler_mode"></param>
        public void RemoveSpecialHandlerMode(Mode special_handler_mode) => Game.Modes.Remove(special_handler_mode);

        /// <summary>
        /// Restarts the search countdown. Won't restart if the ball is on a StopSwitch <para/>
        /// Cancels delay handlers like coil pulsing in ball searching
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        public bool Reset(Switch sw)
        {
            if (_enabled)
            {
                // Stop delayed coil activations in case a ball search has already started
                CancelDelayed(nameof(PopCoil));
                CancelDelayed(nameof(StartSpecialHandlerModes));
                bool schedule_search = true;
                foreach (string swc in stop_switches.Keys)
                {
                    if (sw == null) break;
                    // Don't restart the search countdown if a ball is resting on a stop_switch. First
                    // build the appropriate function call into the switch, then call it.
                    string state_str = stop_switches[swc];
                    if (state_str == "active" && sw.IsActive()) schedule_search = false;
                    if (state_str == "closed" && sw.IsClosed()) schedule_search = false;
                    if (state_str == "open" && sw.IsOpen()) schedule_search = false;
                    if (state_str == "inactive" && sw.IsInactive()) schedule_search = false;
                }
                if (schedule_search)
                {
                    CancelDelayed(nameof(PerformSearch));
                    Delay(nameof(PerformSearch), EventType.None, countdown_time, new BallSearchDelayHandler(PerformSearch));
                }
            }
            return SWITCH_CONTINUE;
        }

        /// <summary>
        /// Add the special mode to the games modes. Adds delay of 7 to remove the mode
        /// </summary>
        public void StartSpecialHandlerModes()
        {
            foreach (Mode special_handler_mode in special_handler_modes)
            {
                Game.Modes.Add(special_handler_mode);
                Delay(nameof(RemoveSpecialHandlerMode), EventType.None, 7, new RemoveSpecialHandlerDelegate(RemoveSpecialHandlerMode), special_handler_mode);
            }
        }

        /// <summary>
        /// Stops the ball_search_countdown delay
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        public bool Stop(Switch sw)
        {
            CancelDelayed(nameof(PerformSearch));
            return SWITCH_CONTINUE;
        }
    }
}
