namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a component that can monitor
    /// a target instance as it loads.
    /// </summary>
    /// <typeparam name="TTarget">The target instance type.</typeparam>
    public interface ILoaderPlugin<TTarget>
    {
        /// <summary>
        /// Signals the beginning of a load.
        /// </summary>
        /// <param name="target">The target being loaded.</param>
        void BeginLoad(TTarget target);

        /// <summary>
        /// Signals the end of a load.
        /// </summary>
        /// <param name="target">The target being loaded.</param>
        void EndLoad(TTarget target);
    }
}