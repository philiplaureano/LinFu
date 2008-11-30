namespace LinFu.Reflection.Plugins
{
    /// <summary>
    /// A class that implements the basic functionality of
    /// a loader plugin.
    /// </summary>
    /// <typeparam name="TTarget">The type being loaded.</typeparam>
    public abstract class BaseLoaderPlugin<TTarget> : ILoaderPlugin<TTarget>
    {
        #region ILoaderPlugin<TTarget> Members

        /// <summary>
        /// Signals the beginning of a load.
        /// </summary>
        /// <param name="target">The target being loaded.</param>
        public virtual void BeginLoad(TTarget target)
        {
        }

        /// <summary>
        /// Signals the end of a load.
        /// </summary>
        /// <param name="target">The target being loaded.</param>
        public virtual void EndLoad(TTarget target)
        {
        }

        #endregion
    }
}