namespace LinFu.AOP.Interfaces
{
    /// <summary>
    ///     Represents a class that can instantiate object instances.
    /// </summary>
    /// <typeparam name="TContext">
    ///     The type that describes the context of the object instantiation.
    /// </typeparam>
    public interface IActivator<TContext>
        where TContext : IActivationContext
    {
        /// <summary>
        ///     Creates an object instance.
        /// </summary>
        /// <param name="context">The context that describes the request to instantiate the target type.</param>
        /// <returns>A valid object instance.</returns>
        object CreateInstance(TContext context);
    }
}