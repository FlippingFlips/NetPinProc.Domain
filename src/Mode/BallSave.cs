using NetPinProc.Domain.PinProc;
using System;

namespace NetPinProc.Domain.Modes
{
    ///<inheritdoc/>
    public delegate void BallSaveEnable(bool enabled);
    ///<inheritdoc/>
    public delegate void BallSaveEventHandler();
    ///<inheritdoc/>
    public delegate int GetNumBallsToSaveHandler();
    /// <summary>
    /// Manages a games ball save functionality by keeping track of the ball save timer and the number of balls to be saved. <para/>
    /// TODO: Add LED support for blinking lamp
    /// </summary>
    public class BallSave : Mode
    {
        /// <summary>
        /// Optional method to be called to tell a trough to save balls. Should be linked externally to an Enable method for a trough
        /// </summary>
        public Delegate TroughEnableBallSave = null;
        /// <summary>
        /// If ball is saved do we save more afterwards
        /// </summary>
        public bool AllowMultipleSaves = false;
        /// <summary>
        /// Optional method to be called when a ball is saved. Should be defined externally
        /// </summary>
        Delegate Callback = null;
        string lamp;
        string led;
        int mode_begin = 0;
        int num_balls_to_save = 1;
        int timer = 0;
        int timer_hold;

        /// <summary>
        /// Creates a ball save, balls saved defaults to 1 <para/>
        /// </summary>
        /// <param name="game"></param>
        /// <param name="lamp">the lamp / led to blink</param>
        /// <param name="delayed_start_switch">Delay switch, after this is hit the ball save starts</param>
        /// <exception cref="NullReferenceException"></exception>
        public BallSave(IGameController game, string lamp, string delayed_start_switch = "")
            : base(game, 3)
        {
            //find the lamp or LED, if none provided throw exception
            if (game.Lamps.ContainsKey(lamp)) 
            {                
                this.lamp = lamp;
                game.Logger.Log($"{nameof(BallSave)}: ball save lamp found={lamp}");
            }
            else
            {
                game.Logger.Log($"{nameof(BallSave)}: no lamp found for {lamp}, looking for led...");
                if (game.LEDS.ContainsKey(lamp))
                {
                    led = lamp;
                    game.Logger.Log($"{nameof(BallSave)}: ball save led found={lamp}");
                }                    
                else 
                { 
                    game.Logger.Log($"{nameof(BallSave)}: no led found for {lamp}, returning, provide a lamp or led"); 
                    throw new NullReferenceException("No led or lamp found for ballsave"); 
                }
            }            

            this.num_balls_to_save = 1;
            this.mode_begin = 0;
            this.AllowMultipleSaves = false;
            this.timer = 0;

            if (delayed_start_switch != "")
                AddSwitchHandler(delayed_start_switch, SwitchHandleType.inactive, 1.0, this.DelayedStartHandler);

            Callback = null;
            TroughEnableBallSave = null;
        }

        /// <summary>
        /// Add time to the ball saver and <see cref="UpdateLamps"/>
        /// </summary>
        /// <param name="add_time"></param>
        /// <param name="allow_multiple_saves"></param>
        public void Add(int add_time, bool allow_multiple_saves = true)
        {
            if (timer >= 1)
            {
                timer += add_time;
                UpdateLamps();
            }
            else
            {
                Start(num_balls_to_save, add_time, true, allow_multiple_saves);
            }
        }

        /// <summary>
        /// Delay the ball Start, let trough know ball save _enabled.
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        public bool DelayedStartHandler(Switch sw)
        {
            Game.Logger.Log($"P-ROC:{nameof(BallSave)}:{nameof(DelayedHandler)}: starting. mode_begin="+mode_begin, LogLevel.Debug);
            if (mode_begin == 1)
            {
                this.timer = timer_hold;
                this.mode_begin = 0;
                this.UpdateLamps();
                CancelDelayed(nameof(TimerCountdown));
                Delay(nameof(TimerCountdown), EventType.None, 1.0, new AnonDelayedHandler(TimerCountdown));
                TroughEnableBallSave?.DynamicInvoke(true);
            }
            return SWITCH_CONTINUE;
        }

        /// <summary>
        /// Disables ball save logic
        /// </summary>
        public void Disable()
        {
            TroughEnableBallSave?.DynamicInvoke(false);
            timer = 0;
            DisableLamp();
        }

        /// <summary>
        /// Disables the lamp / led
        /// </summary>
        public void DisableLamp()
        {
            if (lamp != null)
                Game.Lamps[lamp].Disable();
            else if (led != null)
                Game.LEDS[led].Disable();
        }

        /// <summary>
        /// Returns the number of balls to be saved. Typically this is linked to a trough object so the trough
        /// can decide if a draining ball should be saved.
        /// </summary>
        /// <returns></returns>
        public int GetNumBallsToSave() => num_balls_to_save;

        /// <summary>
        /// Do we have time in the <see cref="timer"/>
        /// </summary>
        /// <returns></returns>
        public bool IsActive() => timer > 0;

        /// <summary>
        /// Disables the ball save logic when multiple saves are not allowed. This is typically linked to a 
        /// trough object so the trough can notify this logic when a ball is being saved.
        /// 
        /// If self.Callback is externally defined, that method will be called from here.
        /// </summary>
        public void LaunchCallback()
        {
            if (!AllowMultipleSaves)
                Disable();

            if (Callback != null)
                Callback.DynamicInvoke();
        }

        ///<inheritdoc/>
        public override void ModeStopped() => Disable();

        /// <summary>
        /// If <see cref="AllowMultipleSaves"/> is disabled then Disable the lamp / led set timer to 1
        /// </summary>
        public void SavingBall()
        {
            if (!AllowMultipleSaves)
            {
                timer = 1;
                DisableLamp();
            }
        }

        /// <summary>
        /// Activates the ball save logic
        /// </summary>
        public void Start(int num_balls_to_save = 1, int time = 12, bool now = true, bool allow_multiple_saves = false)
        {
            this.AllowMultipleSaves = allow_multiple_saves;
            this.num_balls_to_save = num_balls_to_save;

            if (time > this.timer) this.timer = time;
            Game.Logger.Log($"P-ROC:{nameof(BallSave)}:{nameof(Start)}", LogLevel.Debug);
            UpdateLamps();

            if (now)
            {
                CancelDelayed(nameof(TimerCountdown));
                Delay(nameof(TimerCountdown), EventType.None, 1, new AnonDelayedHandler(TimerCountdown));
                TroughEnableBallSave?.DynamicInvoke(true);
            }
            else
            {
                mode_begin = 1;
                timer_hold = time;
            }

        }

        /// <summary>
        /// Starts blinking the ball save lamp. Oftentimes called externally to Start blinking a lamp before
        /// the ball is launched.
        /// </summary>
        public void StartLamp()
        {
            if(lamp != null)
                Game.Lamps[lamp].Schedule(0xFF00FF00, 0, true);
            else if(led!=null)
                Game.LEDS[led].Script(new Pdb.LEDScript[] { new Pdb.LEDScript { Duration = 1000, Colour = new uint[] { 255, 0, 0 } } });
        }

        /// <summary>
        /// Blink slow over 5 secs, blink fast with 2
        /// </summary>
        public override void UpdateLamps()
        {
            if (timer > 5)
                StartLamp();
            else if (timer > 2)
            {
                if (lamp != null)
                    Game.Lamps[lamp].Schedule(0x55555555, 0, true);
                else if (led != null)
                    Game.LEDS[led].Script(new Pdb.LEDScript[] { new Pdb.LEDScript { Duration = 500, Colour = new uint[] { 255, 0, 0 } } });
            }
            else
            {
                DisableLamp();
            }                
        }

        private void TimerCountdown()
        {
            timer--;
            if (timer >= 1)
                Delay(nameof(TimerCountdown), EventType.None, 1.0, new AnonDelayedHandler(TimerCountdown));
            else
                Disable();

            UpdateLamps();
        }
    }
}
