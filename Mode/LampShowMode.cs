using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Mode subclass that manages a single LampShow class updating it in the mode_tick method
    /// </summary>
    public class LampShowMode : Mode, ILampShowMode
    {
        /// <summary>
        /// Method to call when finished
        /// </summary>
        public Delegate Callback;
        /// <summary>
        /// LampShow for this mode
        /// </summary>
        public LampShow Lampshow;
        /// <summary>
        /// Repeat will restart when <see cref="ShowOver"/>
        /// </summary>
        public bool Repeat;
        /// <summary>
        /// Set when time is over for the show
        /// </summary>
        public bool ShowOver;

        /// <inheritdoc/>
        public LampShowMode(IGameController game)
            : base(game, 3)
        {
            this.Lampshow = new LampShow(game);
            this.ShowOver = true;
        }

        /// <inheritdoc/>
        public void Load(string filename, bool repeat = false, Delegate callback = null)
        {
            this.Callback = callback;
            this.Repeat = repeat;
            this.Lampshow.reset();
            this.Lampshow.load(filename);
            this.Restart();
        }
        /// <inheritdoc/>
        public override void ModeTick()
        {
            if (this.Lampshow.is_complete() && !ShowOver)
            {
                if (this.Repeat)
                    this.Restart();
                else
                {
                    this.CancelDelayed("show_tick");
                    this.ShowOver = true;
                    if (this.Callback != null)
                        Callback.DynamicInvoke();
                }
            }
            else if (!ShowOver)
            {
                this.Lampshow.tick();
            }
        }

        /// <inheritdoc/>
        public void Restart()
        {
            this.Lampshow.restart();
            this.ShowOver = false;
        }
    }
}
