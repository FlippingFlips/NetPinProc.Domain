namespace NetPinProc.Domain
{
    /// <inheritdoc/>
    public struct DMDPoint
    {
        /// <summary>
        /// x and y points
        /// </summary>
        public int x, y;
    }

    /// <inheritdoc/>
    public struct DMDSize
    {
        /// <summary>
        /// width and height of DMD
        /// </summary>
        public int width, height;
    }

    /// <inheritdoc/>
    public struct DMDRect
    {
        /// <inheritdoc/>
        public DMDPoint origin;
        /// <inheritdoc/>
        public DMDSize size;
    }

    /// <inheritdoc/>
    public struct DMDFrame
    {
        /// <inheritdoc/>
        public DMDSize size;
        /// <inheritdoc/>
        public byte[] buffer;
    }

    /// <summary>
    /// Blend modes
    /// </summary>
    public enum DMDBlendMode
    {
        /// <inheritdoc/>
        DMDBlendModeCopy = 0,
        /// <inheritdoc/>
        DMDBlendModeAdd = 1,
        /// <inheritdoc/>
        DMDBlendModeSubtract = 2,
        /// <inheritdoc/>
        DMDBlendModeBlackSource = 3,
        /// <inheritdoc/>
        DMDBlendModeAlpha = 4,
        /// <inheritdoc/>
        DMDBlendModeAlphaBoth = 5
    }
}
