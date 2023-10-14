namespace NetPinProc.Domain
{
    /// <summary>
    /// Game item in machine
    /// </summary>
    public interface IGameItem
    {
        /// <inheritdoc/>
        string Name { get; set; }
        /// <inheritdoc/>
        ushort Number { get; set; }
    }
}