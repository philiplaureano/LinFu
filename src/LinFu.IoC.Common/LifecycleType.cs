namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// An enumeration that denotes the instance behavior
    /// of a particular object reference.
    /// </summary>
    public enum LifecycleType
    {
        /// <summary>
        /// This means that a new object instance 
        /// will be created on each call.
        /// </summary>
        OncePerRequest = 0,

        /// <summary>
        /// This means that a new object instance 
        /// will be created only once per thread.
        /// </summary>
        OncePerThread = 1,

        /// <summary>
        /// This means that only a single object instance
        /// will ever be created in spite of the number of
        /// subsequent requests for a new object instance.
        /// </summary>
        Singleton = 2
    }
}