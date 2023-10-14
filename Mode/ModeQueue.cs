using NetPinProc.Domain.PinProc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace NetPinProc.Domain
{
    /// <summary>
    /// A mode queue for <see cref="IMode"/>
    /// </summary>
    public class ModeQueue : IModeQueue
    {
        IGameController _game;
        /// <summary>
        /// Object to lock thread when adding modes
        /// </summary>
        protected object _modeLockObj = new object();
        List<IMode> _modes;

        /// <summary>
        /// Creates new mode list
        /// </summary>
        /// <param name="game"></param>
        public ModeQueue(IGameController game)
        {
            _game = game;
            _modes = new List<IMode>();
        }

        ///<inheritdoc/>
        public List<IMode> Modes
        {
            get { return _modes; }
        }

        ///<inheritdoc/>
        public void Add(IMode mode)
        {
            _game?.Logger.Log(nameof(ModeQueue)+nameof(Add)+": "+mode.GetType().Name, LogLevel.Debug);

            if (_modes.Contains(mode))
                throw new Exception("Attempted to add mode " + mode.ToString() + ", already in mode queue.");

            lock (_modeLockObj)
            {
                _modes.Add(mode);
            }
            //self.modes.sort(lambda x, y: y.priority - x.priority)
            _modes.Sort();
            mode.ModeStarted();

            if (mode == _modes[0])
                mode.ModeTopMost();
        }

        ///<inheritdoc/>
        public void Clear() => _modes.Clear();

        ///<inheritdoc/>
        public void HandleEvent(Event evt)
        {
            IMode[] modes = new IMode[_modes.Count()];
            _modes.CopyTo(modes);
            for (int i = 0; i < modes.Length; i++)
            {
                bool handled = modes[i].HandleEvent(evt);
                if (handled)
                    break;
            }
        }

        ///<inheritdoc/>
        public void Remove(IMode mode)
        {
            mode.ModeStopped();
            lock (_modeLockObj)
            {
                _modes.Remove(mode);
            }

            if (_modes.Count > 0)
                _modes[0].ModeTopMost();
        }

        ///<inheritdoc/>
        public void Tick()
        {
            IMode[] modes;
            lock (_modeLockObj)
            {
                modes = new IMode[_modes.Count()];
                _modes.CopyTo(modes);
            }
            for (int i = 0; i < modes.Length; i++)
            {
                modes[i].DispatchDelayed();
                modes[i].ModeTick();
            }
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            var result = "----MODE QUEUE-----";
            var modes = Modes.OrderByDescending(m => m.Priority).ToArray();
            for (int i = 0; i < modes.Count(); i++) { result += $"{modes[i].ToString()}\n"; }
            return result;
        }
    }
}
