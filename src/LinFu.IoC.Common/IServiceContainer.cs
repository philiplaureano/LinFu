using System;
using System.Collections.Generic;

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
        void AddFactory(string serviceName, Type serviceType, IEnumerable<Type> additionalParameterTypes,
                        IFactory factory);        
    }
}