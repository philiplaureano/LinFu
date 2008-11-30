using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// A strongly-typed version of <see cref="IFactory"/>. Allows users
    /// to create their own service instances
    /// </summary>
    /// <typeparam name="T">The instance type that can be created by this factory.</typeparam>
    public interface IFactory<T>
    {
        /// <summary>
        /// Creates a service instance using the given <see cref="IFactoryRequest"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        T CreateInstance(IFactoryRequest request);
    }
}