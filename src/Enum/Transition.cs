namespace NetPinProc.Domain
{
    /// <summary>
    /// Forward, back or non
    /// </summary>
    public enum Direction
    {
        /// <inheritdoc/>
        Forward,
        /// <inheritdoc/>
        Backward,
        /// <inheritdoc/>
        None
    }

    /// <summary>
    /// Commonly used by DMD transitions
    /// </summary>
    public enum TransitionDirection
    {
        ///<inheritdoc/>
        North,
        ///<inheritdoc/>
        South,
        ///<inheritdoc/>
        East,
        ///<inheritdoc/>
        West
    }

    /// <summary>
    /// vert or horz
    /// </summary>
    public enum TransitionVertical
    {
        /// <inheritdoc/>
        Vertical,
        /// <inheritdoc/>
        Horizontal
    }
}
