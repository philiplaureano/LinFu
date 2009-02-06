using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a type that can create service instances from a given <see cref="IFactory"/> instance and <see cref="IFactoryRequest"/>.
    /// </summary>
    public interface ICreateInstance
    {
        /// <summary>
        /// Creates a service instance using the given <paramref name="factoryRequest"/> and <see cref="IFactory"/> instance.
        /// </summary>
        /// <param name="factoryRequest">The <see cref="IFactoryRequest"/> instance that describes the context of the service request.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be used to instantiate the service type.</param>
        /// <returns>A valid service instance.</returns>
        object CreateFrom(IFactoryRequest factoryRequest, IFactory factory);
    }
}
