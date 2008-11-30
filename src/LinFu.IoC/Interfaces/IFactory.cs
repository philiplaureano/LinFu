using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Allows an object to create its own service instances.
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Creates a service instance using the given <see cref="IFactoryRequest"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        object CreateInstance(IFactoryRequest request);
    }
}