using System;
using System.Collections.Generic;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents the most basic implementation for a service locator.
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Determines whether or not the container can create
        /// the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The type of service used to determine whether or not the given service can actually be created</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <returns>A <see cref="bool">boolean</see> value that indicates whether or not the service exists.</returns>
        bool Contains(Type serviceType, IEnumerable<Type> additionalParameterTypes);

        /// <summary>
        /// Determines whether or not a service can be created using
        /// the given <paramref name="serviceName">service name</paramref>
        /// and <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that the factory type must support.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        bool Contains(string serviceName, Type serviceType, IEnumerable<Type> additionalParameterTypes);

        /// <summary>
        /// Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <param name="serviceType">The service type to instantiate.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a null value.</returns>
        object GetService(Type serviceType, params object[] additionalArguments);

        /// <summary>
        /// Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The service type to instantiate.</param>   
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>     
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        object GetService(string serviceName, Type serviceType, params object[] additionalArguments);

        /// <summary>
        /// Determines whether or not a container will throw an exception
        /// if the requested service is not found.
        /// </summary>
        bool SuppressErrors { get; set; }
    }
}