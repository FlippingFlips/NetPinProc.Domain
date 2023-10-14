using NetPinProc.Domain.PinProc;
using System;
using System.Collections.Generic;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Controller object that encapsulates a LampShow class and helps to restore lamp drivers to their
    /// prior state.
    /// </summary>
    public class LampController : ILampController
    {
        /// <summary>
        /// The show to play when resumed
        /// </summary>
        public string ResumeKey = "";

        /// <summary>
        /// Current resume state
        /// </summary>
        public bool ResumeState = false;

        /// <summary>
        /// True if a Show is currently playing
        /// </summary>
        public bool ShowPlaying;

        /// <summary>
        /// List of Shows to load (key, filepath). 
        /// Used to create LampShow objects
        /// </summary>
        public Dictionary<string, string> Shows;

        private IGameController game;
        private Dictionary<string, SavedLampState> SavedStateDicts;

        /// <summary>
        /// LampShowMode that must be added to the mode queue
        /// </summary>
        ILampShowMode Show = null;

        /// <summary>
        /// Creates a <see cref="LampShowMode"/> and initializes
        /// </summary>
        /// <param name="game"></param>
        public LampController(IGameController game)
        {
            this.game = game;
            this.Show = new LampShowMode(game);
            this.SavedStateDicts = new Dictionary<string, SavedLampState>();
            this.Shows = new Dictionary<string, string>();
        }

        /// <inheritdoc/>
        public void PlayShow(string key, bool repeat = false, Delegate callback = null)
        {
            // Always Stop any previously running Show first
            this.StopShow();
            this.Show.Load(this.Shows[key], repeat, callback);
            this.game.Modes.Add(this.Show);
            this.ShowPlaying = true;
        }

        /// <inheritdoc/>
        public void RegisterShow(string key, string show_file) => this.Shows.Add(key, show_file);

        /// <summary>
        /// Restores playback from <see cref="RestoreState(string)"/> by using the <see cref="ResumeKey"/>
        /// </summary>  
        public void RestorePlayback()
        {
            this.ResumeState = false;
            this.RestoreState(this.ResumeKey);
            //this.callback() ?
        }

        /// <inheritdoc/>
        public void RestoreState(string key)
        {
            if (this.SavedStateDicts.ContainsKey(key))
            {
                int duration = 0;
                SavedLampState state = SavedStateDicts[key];
                foreach (string lamp_name in state.LampStates.Keys)
                {
                    if (!lamp_name.StartsWith("gi0"))
                    {
                        double time_remaining = (state.LampStates[lamp_name].state.OutputDriveTime + state.LampStates[lamp_name].time) - state.TimeSaved;
                        // Disable the lamp if it has never been used or if there would have been
                        // less than 1 second of drive time when the state was saved
                        if ((state.LampStates[lamp_name].time == 0 || time_remaining < 1.0)
                            && state.LampStates[lamp_name].state.OutputDriveTime != 0)
                            this.game.Lamps[lamp_name].Disable();
                        else
                        {
                            // Otherwise, resume the lamp
                            if (state.LampStates[lamp_name].state.OutputDriveTime == 0)
                            {
                                duration = 0;
                            }
                            else
                            {
                                duration = (int)time_remaining;
                            }
                            if (state.LampStates[lamp_name].state.Timeslots == 0)
                                this.game.Lamps[lamp_name].Disable();
                            else
                            {
                                this.game.Lamps[lamp_name].Schedule(state.LampStates[lamp_name].state.Timeslots,
                                    duration,
                                    state.LampStates[lamp_name].state.WaitForFirstTimeSlot);
                            }
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void SaveState(string key)
        {
            SavedLampState state = new SavedLampState();
            state.TimeSaved = Time.GetTime();
            foreach (IDriver lamp in this.game.Lamps.Values)
            {
                state.LampStates.Add(lamp.Name, new LampStateRecord(lamp.LastTimeChanged, lamp.State));
            }

            if (this.SavedStateDicts.ContainsKey(key))
                this.SavedStateDicts[key] = state;
            else
                this.SavedStateDicts.Add(key, state);
        }

        /// <summary>
        /// Stops the Show <see cref="ShowPlaying"/>. If <see cref="ShowPlaying"/>. The <see cref="Show"/> is removed from the <see cref="IGameController.Modes"/>
        /// </summary>
        public void StopShow()
        {
            if (ShowPlaying)
                this.game.Modes.Remove(this.Show);
            this.ShowPlaying = false;
        }

        struct LampStateRecord
        {
            public DriverState state;
            public double time;
            public LampStateRecord(double time, DriverState state)
            {
                this.time = time;
                this.state = state;
            }
        }

        class SavedLampState
        {
            public double TimeSaved;
            public Dictionary<string, LampStateRecord> LampStates { get; set; }
        }
    }
}
