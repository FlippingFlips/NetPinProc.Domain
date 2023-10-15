using NetPinProc.Domain.PinProc;
using System;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Represents a game mode
    /// </summary>
    public interface IMode : IComparable
    {
        /// <summary>
        /// Reference to the hosting GameController object / descendant
        /// </summary>
        IGameController Game { get; set; }
        /// <summary>
        /// The priority of the mode in the queue
        /// </summary>
        int Priority { get; set; }
        /// <summary>
        /// The display layer for the mode
        /// </summary>
        ILayer Layer { get; set; }
        /// <summary>
        /// Adds a switch handler to the AcceptedSwitch list for the mode
        /// </summary>
        /// <param name="Name">Switch Name</param>
        /// <param name="handleType"><see cref="SwitchHandleType"/></param>
        /// <param name="Delay">float number of seconds that the state should be held before invoking the handler, 
        /// or None if it should be invoked immediately.</param>
        /// <param name="Handler">The handler to invoke</param>
        void AddSwitchHandler(string Name, SwitchHandleType handleType = SwitchHandleType.active, double Delay = 0, SwitchAcceptedHandler Handler = null);
        /// <summary>
        /// Cancel a delayed event
        /// </summary>
        /// <param name="Name">The name of the delay to cancel</param>
        void CancelDelayed(string Name);
        /// <summary>
        /// Delay an event for the specified period of time
        /// </summary>
        /// <param name="Name">The name of the delayed event</param>
        /// <param name="Event_Type">The type of event to delay</param>
        /// <param name="Delay">The delay in ms before the callback is fired</param>
        /// <param name="Handler">The callback to fire</param>
        /// <param name="Param">The parameters to the given callback</param>
        void Delay(string Name, EventType Event_Type, double Delay, Delegate Handler, object Param = null);
        /// <summary>
        /// Called by the GameController to dispatch any delayed events
        /// </summary>
        void DispatchDelayed();
        /// <summary>
        /// Handles a switch event
        /// <para/>
        /// This is called each time that an event is read in from the controller board
        /// </summary>
        /// <param name="evt">The event and type that was read in from the PROC</param>
        /// <returns>true if the event was handled and should not be propagated, false to propagate to other modes</returns>
        bool HandleEvent(Event evt);
        /// <summary>
        /// Notifies the mode that it is now active on the mode queue.<para/>
        /// This method should not be invoked directly; it is called by the GameController run loop
        /// </summary>
        void ModeStarted();
        /// <summary>
        /// Notifies the mode that it has been removed from the mode queue <para/>
        /// This method should not be invoked directly. It is called by the GameController run loop
        /// </summary>
        void ModeStopped();
        /// <summary>
        /// Called by the game controller run loop during each loop when the mode is running
        /// </summary>
        void ModeTick();
        /// <summary>
        /// Notifies the mode that it is now the topmost mode on the mode queue <para/>
        /// This method should not be invoked directly, it is called by the GameController run loop
        /// </summary>
        void ModeTopMost();
        /// <summary>
        /// Type name and <see cref="Priority"/>
        /// </summary>
        /// <returns></returns>
        string ToString();
        /// <summary>
        /// Called by the GameController to re-apply active lamp schedules
        /// </summary>
        void UpdateLamps();
    }
}