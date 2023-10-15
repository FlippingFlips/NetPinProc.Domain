using NetPinProc.Domain.PinProc;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Software driver
    /// </summary>
    public interface IVirtualDriver : IDriver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>_currentState, for simulation this is only place set?</returns>
        bool GetCurrentState();
        /// <summary>
        /// Changes the state
        /// </summary>
        /// <param name="newState"></param>
        void ChangeState(bool newState);
        /// <summary>
        /// Increments schedule
        /// </summary>
        void IncSchedule();
        /// <summary>
        /// Generic state change request that represents the P-ROC's PRDriverUpdateState function
        /// </summary>
        /// <param name="newState"></param>
        void UpdateState(DriverState newState);
    }
}