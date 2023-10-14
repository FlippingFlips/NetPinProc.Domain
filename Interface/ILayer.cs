namespace NetPinProc.Domain
{
    /// <inheritdoc/>
    public interface ILayer
    {
        /// <summary>
        /// If false, the DisplayController will ignore this layer
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// Determines whether layers below this one will be rendered.
        /// If 'true', the DisplayController will not render any layers after this one
        /// (such as from modes with lower priorities)
        /// </summary>
        bool Opaque { get; set; }
        /// <summary>
        /// Composites the next Frame of this layer onto the given target buffer.
        /// Called by DisplayController.update()
        /// Generally subclasses should not override this method. Implement NextFrame() instead
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IFrame CompositeNext(IFrame target);
        /// <summary>
        /// Returns an instance of a Frame object to be shown or null if there is no Frame.
        /// The default implementation returns null, subclasses should implement this method.
        /// </summary>
        IFrame NextFrame();
        /// <summary>
        /// Reset the layer
        /// </summary>
        void Reset();
        /// <summary>
        /// Sets the TargetX and TargetY position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetTargetPosition(int x, int y);
    }
}