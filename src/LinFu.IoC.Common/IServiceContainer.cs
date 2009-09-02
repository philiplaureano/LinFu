using System;
using System.Collections.Generic;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// An inversion of control container that supports
    /// named services.
    /// </summary>
    /// <seealso name="IContainer"/>
    public interface IServiceContainer : IContainer
    {
        /// <summary>
        /// The list of preprocessors that will handle
        /// every service request before each actual service is created.
        /// </summary>
        IList<IPreProcessor> PreProcessors { get; }

        /// <summary>
        /// The list of postprocessors that will handle every
        /// service request result.
        /// </summary>
        IList<IPostProcessor> PostProcessors { get; }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        void AddFactory(string serviceName, Type serviceType, IEnumerable<Type> additionalParameterTypes, IFactory factory);

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
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The service type to instantiate.</param>   
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>     
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        object GetService(string serviceName, Type serviceType, params object[] additionalArguments);
    }
}