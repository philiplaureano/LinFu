using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Extends the <see cref="IFactory"/> instance with a few helper methods.
    /// </summary>
    public static class FactoryExtensions
    {
        /// <summary>
        /// Creates an object instance.
        /// </summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="serviceType">The requested service type.</param>
        /// <param name="container">The target service contaienr.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to create the service instance.</param>
        /// <returns>A service instance.</returns>
        public static object CreateInstance(this IFactory factory, Type serviceType, 
            IServiceContainer container, params object[] additionalArguments)
        {
            var request = new FactoryRequest()
                              {
                                  ServiceName = null,
                                  ServiceType = serviceType,
                                  Arguments = additionalArguments
                              };

            return factory.CreateInstance(request);
        }
    }
}
