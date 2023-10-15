namespace NetPinProc.Domain
{
    /// <summary>
    /// Display Frame, used by DMD
    /// </summary>
    public interface IFrame
    {
        /// <inheritdoc/>
        DMDFrame Frame { get; set; }
        /// <summary>
        /// Width of frame
        /// </summary>
        int Width { get; }
        /// <summary>
        /// Height of frame
        /// </summary>
        int Height { get; }
        /// <summary>
        /// Gets a copy of this frame
        /// </summary>
        /// <returns></returns>
        IFrame Copy();
        /// <summary>
        /// Generates a new Frame based on a sub rectangle of this Frame
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        IFrame SubFrame(int x, int y, int width, int height);
    }
}