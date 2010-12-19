using System;
using System.Collections.Generic;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// An inversion of control container interface.
    /// </summary>
    public interface IContainer : IServiceLocator
    {
        /// <summary>
        /// The list of services currently available inside the container.
        /// </summary>
        IEnumerable<IServiceInfo> AvailableServices { get; }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The service type to associate with the factory</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be responsible for creating the service instance</param>
        void AddFactory(Type serviceType, IEnumerable<Type> additionalParameterTypes, IFactory factory);
    }
}