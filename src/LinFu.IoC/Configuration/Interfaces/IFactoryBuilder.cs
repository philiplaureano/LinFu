using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a class that can generate <see cref="IFactory"/> instances 
    /// from a given service type, implementing type, and lifecycle.
    /// </summary>
    public interface IFactoryBuilder
    {
        /// <summary>
        /// Generates a <see cref="IFactory"/> instance that can create the <paramref name="serviceType"/>
        /// using the <paramref name="implementingType"/> and <paramref name="lifecycle"/> model.
        /// </summary>
        /// <param name="serviceType">The service type that will be created by the factory.</param>
        /// <param name="implementingType">The concrete type that will provide the implementation for the service type.</param>
        /// <param name="lifecycle">The instancing behavior of the given service type.</param>
        /// <returns>A valid <see cref="IFactory"/> instance.</returns>
        IFactory CreateFactory(Type serviceType, Type implementingType, LifecycleType lifecycle);
    }
}
