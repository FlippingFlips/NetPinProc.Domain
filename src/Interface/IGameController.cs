using NetPinProc.Domain.PinProc;
using System.Collections.Generic;
using System.Threading;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Core object representing the game itself.
    /// </summary>
    public interface IGameController
    {
        /// <summary>
        /// The number of the current ball. A value of 1 represents the first ball; 0 indicates game over.
        /// </summary>
        int Ball { get; }
        /// <summary>
        /// All coils/solenoids within the game
        /// </summary>
        AttrCollection<ushort, string, IDriver> Coils { get; set; }
        /// <summary>
        /// Current machine configuration representation
        /// </summary>
        MachineConfiguration Config { get; }
        /// <summary>
        /// Current player playing
        /// </summary>
        int CurrentPlayerIndex { get; }
        /// <summary>
        /// Are the flippers enabled?
        /// </summary>
        bool FlippersEnabled { get; }
        /// <summary>
        /// All GI drivers within the game
        /// </summary>
        AttrCollection<ushort, string, IDriver> GI { get; set; }
        /// <summary>
        /// The lamp controller with lampshows
        /// </summary>
        ILampController LampController { get; set; }
        /// <summary>
        /// All lamps within the game
        /// </summary>
        AttrCollection<ushort, string, IDriver> Lamps { get; set; }
        /// <summary>
        /// All leds within the game
        /// </summary>
        AttrCollection<ushort, string, LED> LEDS { get; set; }
        /// <summary>
        /// Public logging interface class. Make sure all games have a class that implements this interface
        /// </summary>
        ILogger Logger { get; set; }
        /// <summary>
        /// The current list of modes that are active in the game
        /// </summary>
        IModeQueue Modes { get; set; }
        /// <summary>
        /// The list of players currently playing the game
        /// </summary>
        List<IPlayer> Players { get; set; }
        /// <summary>
        /// PROC device driver wrapper
        /// </summary>
        IProcDevice PROC { get; }
        /// <summary>
        /// All switches and optos within the game
        /// </summary>
        AttrCollection<ushort, string, Switch> Switches { get; set; }
        /// <summary>
        /// Invokes <see cref="CreatePlayer"/> to create default name with Player, then adds a new player to 'Players'
        /// </summary>
        /// <returns></returns>
        IPlayer AddPlayer();
        /// <summary>
        /// Adds points to the current player
        /// </summary>
        void AddPoints(long points);
        /// <summary>
        /// Adds bonus to the current player
        /// </summary>
        void AddBonus(long bonus);
        /// <summary>
        /// Called by the game framework when the current ball has ended
        /// </summary>
        void BallEnded();
        /// <summary>
        /// Called by the game framework when a new ball is starting
        /// </summary>
        void BallStarting();
        /// <summary>
        /// Creates a new player with a given name
        /// </summary>
        /// <param name="name">The name for the player to use, usually auto generated</param>
        /// <returns>A new player object</returns>
        IPlayer CreatePlayer(string name);
        /// <summary>
        /// Returns the current 'Player' object according to the current_player_index value
        /// </summary>
        /// <returns></returns>
        IPlayer CurrentPlayer();
        /// <summary>
        /// Called by the GameController when a DMD event has been received.
        /// </summary>
        void DmdEvent();
        /// <summary>
        /// Enables the flippers via switch rules linked to Main and Hold coils of the same name <para/>
        /// This will also disable any coil Item types that are bumper, eg: slings and bumpers <para/>
        /// If the player tilted the game then you would want all coils to disable. <para/>
        /// Any flippers that are in the PRFlippers collection will be added and coils with same name should match.
        /// </summary>
        /// <param name="enable">Enable or Disable switch rules for flippers</param>
        /// <param name="pulseTime">Pulse time for main coil</param>
        void EnableFlippers(bool enable = true, byte pulseTime = 34);
        /// <summary>
        /// Called by the implementor to notify the game that the current ball has ended
        /// </summary>
        void EndBall();
        /// <summary>
        /// Called by the implementor to notify the game that the game as ended. Should call GameEnded
        /// </summary>
        void EndGame();
        /// <summary>
        /// Set the exit condition for the run loop causing the game to terminate
        /// </summary>
        void EndRunLoop();
        /// <summary>
        /// Called by the GameController when the current game has ended
        /// </summary>
        void GameEnded();
        /// <summary>
        /// Called by the GameController when a new game is starting. Creates new list of players
        /// </summary>
        void GameStarted();
        /// <summary>
        /// The ball time for the current player
        /// </summary>
        /// <returns>The ball time (in seconds) that the current ball has been in play</returns>
        double GetBallTime();
        /// <summary>
        /// Retrieve all events from the PROC interface board
        /// </summary>        
        /// <param name="dmdEvents">include DMD events? 16 added</param>
        /// <returns>A list of events from the PROC</returns>
        Event[] GetEvents(bool dmdEvents = true);
        /// <summary>
        /// The game time for the given player index
        /// </summary>
        /// <param name="player">The player index to calculate the game time for</param>
        /// <returns>The time in seconds the player has been playing the entire game</returns>
        double GetGameTime(int player);
        /// <summary>
        /// Updates switch rules for flipper linked coils. Main flipper is pulsed and the hold with be pulsed with 0 <para/>
        /// </summary>
        /// <param name="switch_name"></param>
        /// <param name="linked_coils">Flipper coil names Main and Hold. eg: flipperLwLMain, flipperLwLHold</param>
        /// <param name="pulseMain"></param>
        void LinkFlipperSwitch(string switch_name, string[] linked_coils, byte pulseMain = 34);
        /// <summary>
        /// Create a new machine configuration representation in memory from a json file on disk.
        /// </summary>
        /// <param name="PathToFile">The path to the configuration json file</param>
        void LoadConfig(string PathToFile);
        /// <summary>
        /// Process the retrieved event from the PROC interface board (switch/dmd events)
        /// </summary>
        /// <param name="evt">The event to process</param>
        void ProcessEvent(Event evt);
        /// <summary>
        /// Reset the game state to normal (like a slam tilt). Clear players, layers and modes. 
        /// </summary>
        void Reset();
        /// <summary>
        /// Should run a game loop, getting events, ticking drivers and modes
        /// </summary>
        void RunLoop(byte delay = 0, CancellationTokenSource cancellationToken = default);
        /// <summary>
        /// Safely disable a coil from another thread
        /// </summary>
        /// <param name="coilName">The coil name to disable</param>
        void SafeDisableCoil(string coilName);
        /// <summary>
        /// Safely drive a coil from another thread
        /// </summary>
        /// <param name="coilName">The coil name to drive</param>
        /// <param name="pulse_time">The time (ms) to pulse (default = 30ms)</param>
        void SafeDriveCoil(string coilName, ushort pulse_time = 30);
        /// <summary>
        /// Save the ball start time into local memory
        /// </summary>
        void SaveBallStartTime();
        /// <summary>
        /// Called by the game framework when a new ball is starting which was the result of a stored extra ball. <para/>
        /// The default implementation calls <see cref="BallStarting"/> which is not called by the framework in this case.
        /// </summary>
        void ShootAgain();
        /// <summary>
        /// Called by the implementor to notify the game that the first ball should be started.
        /// </summary>
        void StartBall();
        /// <summary>
        /// Called by the implementor to notify the game that the game has started.
        /// </summary>
        void StartGame();
        /// <summary>
        /// 
        /// </summary>
        void Tick();
        /// <summary>
        /// Propagate Tick events to all lamps, coils and leds
        /// </summary>
        void TickVirtualDrivers();
        /// <summary>
        /// 
        /// </summary>
        void UpdateLamps();
    }
}