namespace NetPinProc.Domain
{
    /// <summary>
    /// Base Player
    /// </summary>
    public interface IPlayer
    {
        ///<inheritdoc/>
        int ExtraBalls { get; set; }
        ///<inheritdoc/>
        double GameTime { get; set; }
        ///<inheritdoc/>
        string Name { get; set; }
        ///<inheritdoc/>
        long Score { get; set; }
    }
}