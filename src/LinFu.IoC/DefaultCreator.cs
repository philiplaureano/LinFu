using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents the default implementation for the <see cref="ICreateInstance"/>
    /// </summary>
    public class DefaultCreator : ICreateInstance
    {
        /// <summary>
        /// Creates a service instance using the given <paramref name="factoryRequest"/> and <see cref="IFactory"/> instance.
        /// </summary>
        /// <param name="factoryRequest">The <see cref="IFactoryRequest"/> instance that describes the context of the service request.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be used to instantiate the service type.</param>
        /// <returns>A valid service instance.</returns>
        public virtual object CreateFrom(IFactoryRequest factoryRequest, IFactory factory)
        {
            object instance = null;

            // Generate the service instance
            if (factory != null)
                instance = factory.CreateInstance(factoryRequest);

            return instance;
        }
    }
}
