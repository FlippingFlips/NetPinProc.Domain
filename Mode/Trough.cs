using NetPinProc.Domain;
using NetPinProc.Domain.PinProc;
using System;

namespace NetPinProc.Domain.Modes
{
    /// <summary>
    /// Manages trough by providing the following functionality: <para/>
    ///     - Keeps track of the number of balls in play<para/>
    ///     - Keeps track of the number of balls in the trough<para/>
    ///     - Launches one or more balls on request and calls a LaunchCallback when complete (if exists)<para/>
    ///     - Auto-launches balls while ball save is active (if linked to a ball save object)<para/>
    ///     - Identifies when balls drain and calls a registered DrainCallback, if one exists.<para/>
    ///     - Maintains a count of balls locked in playfield lock features (if externally incremented)<para/> 
    ///       and adjusts the count of number of balls in play appropriately<para/>
    /// </summary>
    public class Trough : Mode
    {
        /// <summary>
        /// On Ball saved
        /// </summary>
        public Delegate BallSaveCallback = null;
        /// <summary>
        /// On Ball drained
        /// </summary>
        public Delegate DrainCallback;
        /// <summary>
        /// 
        /// </summary>
        public Delegate LaunchCallback = null;
        /// <summary>
        /// Number of balls in play
        /// </summary>
        public int NumBallsInPlay;

        /// <summary>
        /// 
        /// </summary>
        public Delegate NumBallsToSaveCallback = null;
        bool ball_save_active = false;        
        string[] early_save_switchnames;
        string eject_coilname;
        string eject_switchname;
        bool launch_in_progress = false;
        int num_balls_locked;
        int num_balls_to_launch;
        int num_balls_to_stealth_launch;
        string[] position_switchnames;
        string shooter_lane_switchname;

        /// <summary>
        /// Adds switch handlers for the <see cref="position_switchnames"/> and <see cref="early_save_switchnames"/>
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position_switchnames"></param>
        /// <param name="eject_switchname">must be provided. If this switch is on then the ball is ready to be ejected. See early saves</param>
        /// <param name="eject_coilname"></param>
        /// <param name="early_save_switchnames"></param>
        /// <param name="shooter_lane_switchname"></param>
        /// <param name="drain_callback"></param>
        public Trough(IGameController game, string[] position_switchnames, string eject_switchname, string eject_coilname,
            string[] early_save_switchnames, string shooter_lane_switchname, Delegate drain_callback = null)
            : base(game, 90)
        {
            this.position_switchnames = position_switchnames;
            this.eject_switchname = eject_switchname;
            this.eject_coilname = eject_coilname;
            this.early_save_switchnames = early_save_switchnames;
            this.shooter_lane_switchname = shooter_lane_switchname;
            this.DrainCallback = drain_callback;

            LogChecks();

            if (position_switchnames == null || position_switchnames.Length == 0) { game?.Logger?.Log("NO TROUGH POSITION SWITCHES FOUND, EXIT TROUGH SETUP", LogLevel.Warning); return; }
            if(string.IsNullOrWhiteSpace(shooter_lane_switchname)) { game?.Logger?.Log("NO SHOOTER LANE SWITCH FOUND, EXIT TROUGH SETUP", LogLevel.Warning); return; }

            // Install switch handlers
            foreach (string sw in position_switchnames)
            {
                AddSwitchHandler(sw, SwitchHandleType.active, 0, new SwitchAcceptedHandler(PositionSwitchHandler));
                AddSwitchHandler(sw, SwitchHandleType.inactive, 0, new SwitchAcceptedHandler(PositionSwitchHandler));
            }
            // Install early save switch handlers
            foreach (string sw in early_save_switchnames)
            {
                AddSwitchHandler(sw, SwitchHandleType.active, 0, new SwitchAcceptedHandler(EarlySaveSwitchHandler));
            }

            // Reset all variables
            NumBallsInPlay = 0;
            num_balls_locked = 0;
            num_balls_to_launch = 0;
            num_balls_to_stealth_launch = 0;
            launch_in_progress = false;
            ball_save_active = false;
            BallSaveCallback = null;
            NumBallsToSaveCallback = null;
            LaunchCallback = null;

            game.Logger?.Log(nameof(Trough) + ": Mode created", LogLevel.Debug);
        }

        /// <summary>
        /// Creates a Trough Mode (see <see cref="Trough"/>) from tagged items in the machine configuration. <para/>
        /// Trough switch positions are tagged with 'trough' and the eject coil should be 'coil' <para/>
        /// Shooter lane / Plunger lane tagged with 'shooterLane`
        /// Switches tagged with 'early' will be added to early ball saves if the `troughEject` tag is found
        /// </summary>
        /// <param name="game"></param>
        /// <param name="drain_callback"></param>
        public Trough(IGameController game, Delegate drain_callback = null) : this(game, game?.Config?.GetNamesFromTag("trough", MachineItemType.Switch),
            game?.Config?.GetNameFromTag("troughEject", MachineItemType.Switch), game?.Config?.GetNameFromTag("trough", MachineItemType.Coil),
            game?.Config?.GetNamesFromTag("early", MachineItemType.Switch), game?.Config?.GetNameFromTag("shooterLane", MachineItemType.Switch), drain_callback)
        { }

        /// <summary>
        /// Sets the <see cref="ball_save_active"/> to _enabled
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableBallSave(bool enabled = true) => ball_save_active = enabled;

        /// <summary>
        /// Check whether or not the trough has all balls
        /// </summary>
        /// <returns>True if all balls are in the trough</returns>
        public bool IsFull() => this.NumBalls() == Game.Config.PRGame.NumBalls;

        /// <summary>
        /// Launches balls into play
        /// </summary>
        /// <param name="num">The number of balls to be launch. If ball launches are pending from a previous request
        /// this number will be added to the previously requested number.</param>
        /// <param name="callback">If specified, the Callback will be called once all of the requested balls have been launched.</param>
        /// <param name="stealth">Set to true if the balls being launched should NOT be added to the number of balls in play.
        /// For instance, if a ball is being locked on the playfield and a new ball is being launched to replace it as the active ball
        /// then stealth should be true</param>
        public void LaunchBalls(int num, Delegate callback = null, bool stealth = false)
        {
            num_balls_to_launch += num;
            if (stealth)
                num_balls_to_stealth_launch += num;

            if (!launch_in_progress)
            {
                launch_in_progress = true;
                if (callback != null)
                    LaunchCallback = callback;

                CommonLaunchCode();
            }
        }

        ///<inheritdoc/>
        public override void ModeStopped() => CancelDelayed(nameof(CheckSwitches));

        /// <summary>
        /// Returns the number of balls in the trough by counting the trough switches that are active
        /// </summary>
        /// <returns>The number of balls in the trough</returns>
        public int NumBalls()
        {
            int ball_count = 0;
            foreach (string sw in position_switchnames)
            {
                if (Game.Switches[sw].IsActive())
                    ball_count++;
            }
            return ball_count;
        }

        private void CheckSwitches()
        {
            if (NumBallsInPlay > 0)
            {
                // Base future calculations on how many balls the machine thinks are
                // currently installed
                int num_current_machine_balls = Game.Config.PRGame.NumBalls;
                int temp_num_balls = NumBalls();
                if (ball_save_active)
                {
                    int num_balls_to_save = 0;
                    if (NumBallsToSaveCallback != null)
                        num_balls_to_save = (int)NumBallsToSaveCallback?.DynamicInvoke();

                    // Calculate how many balls shouldn't be in the trough assuming one just drained
                    int num_balls_out = num_balls_locked + (num_balls_to_save - 1);

                    // Translate that to how many balls should be in the trough if one is being saved
                    int balls_in_trough = num_current_machine_balls - num_balls_out;

                    if (temp_num_balls - num_balls_to_launch >= balls_in_trough)
                        LaunchBalls(1, BallSaveCallback, true);
                    else
                        // If there are too few balls in the trough, ignore this one in an attempt to
                        // correct the tracking
                        return;
                }
                else
                {
                    // Calculate how many balls should be in the trough for various condition
                    int num_trough_balls_if_ball_ending = num_current_machine_balls - num_balls_locked;
                    int num_trough_balls_if_multiball_ending = num_trough_balls_if_ball_ending - 1;
                    int num_trough_balls_if_multiball_drain = num_trough_balls_if_ball_ending - (NumBallsInPlay - 1);

                    // The ball should end if all of the balls are in the trough
                    if (temp_num_balls == num_current_machine_balls ||
                        temp_num_balls == num_trough_balls_if_ball_ending)
                    {
                        NumBallsInPlay = 0;
                        DrainCallback?.DynamicInvoke();
                    }

                    // Multiball is ending if all but 1 ball are in the trough....
                    else if (temp_num_balls == num_trough_balls_if_multiball_ending)
                    {
                        NumBallsInPlay = 1;
                        DrainCallback?.DynamicInvoke();
                    }

                    // Otherwise, another ball from multiball is draining if the trough gets one more than
                    // it would have if all NumBallsInPlay are not in the trough
                    else if (temp_num_balls == num_trough_balls_if_multiball_drain)
                    {
                        // Fix NumBallsInPlay if too low
                        if (NumBallsInPlay < 3)
                            NumBallsInPlay = 2;
                        // Otherwise, subtract 1
                        else
                            NumBallsInPlay--;

                        DrainCallback?.DynamicInvoke();
                    }
                }
            }
        }

        /// <summary>
        /// This is the part of the ball launch code that repeats for multiple launches
        /// </summary>
        private void CommonLaunchCode()
        {
            // Only kick out another ball if the last ball is gone from the shooter lane
            if (Game.Switches[shooter_lane_switchname].IsInactive())
            {
                num_balls_to_launch -= 1;
                Game.Coils[eject_coilname].Pulse(40);
                Game.Logger.Log(nameof(Trough) + ": trough ejected ball", LogLevel.Debug);

                // Only increment NumBallsInPlay if there are no more stealth launches to complete.
                if (num_balls_to_stealth_launch > 0)
                    num_balls_to_stealth_launch--;
                else
                    NumBallsInPlay++;

                // If more balls need to be launched, delay 1 second
                if (num_balls_to_launch > 0)
                    Delay("launch", EventType.None, 1.0, new AnonDelayedHandler(CommonLaunchCode));
                else
                {
                    launch_in_progress = false;
                    Game.Logger.Log(nameof(Trough) + ": launch callbacks ended", LogLevel.Debug);
                    LaunchCallback?.DynamicInvoke();                    
                }
            }
            // Otherwise, wait 1 second before trying again
            else
            {
                Delay("launch", EventType.None, 1.0, new AnonDelayedHandler(CommonLaunchCode));
                Game.Logger.Log(nameof(Trough) + ": plungerLane switch blocking trough exit", LogLevel.Debug);
            }
        }

        private bool EarlySaveSwitchHandler(Switch sw)
        {
            Game.Logger.Log("EARLY BALLSAVE: Active " + ball_save_active.ToString(), LogLevel.Debug);
            if (ball_save_active)
            {
                // Only do an early ball save if a ball is ready to be launched
                // otherwise, let the trough switches handle it
                Game.Logger.Log("EJECT COIL ACTIVE: " + Game.Switches[eject_switchname].IsActive().ToString(), LogLevel.Debug);
                if (NumBalls() > 0)
                {
                    LaunchBalls(1, BallSaveCallback, true);
                }
            }
            return SWITCH_CONTINUE;
        }

        /// <summary>
        /// Print checks of the switch items passed to the Trough
        /// </summary>
        private void LogChecks()
        {
            if (position_switchnames?.Length <= 0)
                Game.Logger?.Log(nameof(Trough) + ": no switches are tagged with trough.", LogLevel.Warning);
            else
            {
                if (early_save_switchnames?.Length <= 0) Game.Logger?.Log(nameof(Trough) + ": no early save switches are tagged with 'early'.", LogLevel.Warning);
                else Game.Logger?.Log(nameof(Trough) + ": early save switches found", LogLevel.Debug);

                if (string.IsNullOrWhiteSpace(shooter_lane_switchname)) Game.Logger?.Log(LogLevel.Warning, nameof(Trough) + ": no shooter lane switch found, tag with 'shooterLane'.");
                else Game.Logger?.Log(nameof(Trough) + ": shooter lane switch found " + shooter_lane_switchname, LogLevel.Debug);

                if (string.IsNullOrWhiteSpace(eject_switchname)) Game.Logger?.Log(nameof(Trough) + ": no trough eject switch found, tag with 'troughEject'.", LogLevel.Warning);
                else Game.Logger?.Log(nameof(Trough) + ": trough eject switch found " + eject_switchname, LogLevel.Debug);

                if (string.IsNullOrWhiteSpace(eject_coilname)) Game.Logger?.Log(nameof(Trough) + ": no eject coil trough, tag the coil with 'trough'.", LogLevel.Warning);
                else Game.Logger?.Log(nameof(Trough) + ": trough coil found " + eject_coilname, LogLevel.Debug);
            }
        }

        /// <summary>
        /// Each time a trough position is active/inactive this will be called to set a delay to call <see cref="CheckSwitches"/>.
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        private bool PositionSwitchHandler(Switch sw)
        {
            if(Game.Logger.LogLevel == LogLevel.Verbose) 
            {
                Game.Logger?.Log(nameof(Trough) + ": " + sw.Name + " . checking switches from " + nameof(CheckSwitches), LogLevel.Verbose);
            }            
            CancelDelayed(nameof(CheckSwitches));
            Delay(nameof(CheckSwitches), EventType.None, 0.50, new AnonDelayedHandler(CheckSwitches));
            //CheckSwitches();
            return SWITCH_CONTINUE;
        }
    }
}
