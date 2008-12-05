using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// An extension class that adds a few helper methods to the
    /// <see cref="IFactoryStorage"/> interface.
    /// </summary>
    public static class FactoryStorageExtensions
    {
        /// <summary>
        /// Adds a factory to the current <see cref="IFactoryStorage"/> instance.
        /// </summary>
        /// <param name="storage">The <see cref="IFactoryStorage"/> object that will store the target factory.</param>
        /// <param name="serviceName">The name that will be associated with the target factory.</param>
        /// <param name="serviceType">The service type that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public static void AddFactory(this IFactoryStorage storage, string serviceName,
            Type serviceType, IEnumerable<Type> additionalParameterTypes, IFactory factory)
        {
            var info = new ServiceInfo(serviceName, serviceType, additionalParameterTypes);
            storage.AddFactory(info, factory);
        }

        /// <summary>
        /// Determines which factories should be used
        /// for a particular service request.
        /// </summary>
        /// <param name="storage">The <see cref="IFactoryStorage"/> object that holds the target factory.</param>
        /// <param name="serviceName">The name that will be associated with the target factory.</param>
        /// <param name="serviceType">The service type that the factory will be able to create.</param>
        /// <param name="additionalParameters">The list of additional parameter values that this factory type will use to instantiate the service.</param>
        /// <returns>A factory instance.</returns>
        public static IFactory GetFactory(this IFactoryStorage storage, string serviceName, Type serviceType,
            IEnumerable<object> additionalParameters)
        {
            var additionalParameterTypes = from arg in additionalParameters
                                           let argType = arg != null ? arg.GetType() : typeof(object)
                                           select argType;

            var info = new ServiceInfo(serviceName, serviceType, additionalParameterTypes);
            return storage.GetFactory(info);
        }
       
        /// <summary>
        /// Determines which factories should be used
        /// for a particular service request.
        /// </summary>
        /// <param name="storage">The <see cref="IFactoryStorage"/> object that holds the target factory.</param>
        /// <param name="serviceName">The name that will be associated with the target factory.</param>
        /// <param name="serviceType">The service type that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <returns>A factory instance.</returns>
        public static IFactory GetFactory(this IFactoryStorage storage, string serviceName, Type serviceType,
            IEnumerable<Type> additionalParameterTypes)
        {
            var info = new ServiceInfo(serviceName, serviceType, additionalParameterTypes);
            return storage.GetFactory(info);
        }

        /// <summary>
        /// Determines whether or not a factory exists in storage.
        /// </summary>
        /// <param name="storage">The <see cref="IFactoryStorage"/> object that holds the target factory.</param>
        /// <param name="serviceName">The name that will be associated with the target factory.</param>
        /// <param name="serviceType">The service type that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <returns>Returns <c>true</c> if the factory exists; otherwise, it will return <c>false</c>.</returns>
        public static bool ContainsFactory(this IFactoryStorage storage, string serviceName, Type serviceType,
            IEnumerable<Type> additionalParameterTypes)
        {
            var info = new ServiceInfo(serviceName, serviceType, additionalParameterTypes);
            return storage.ContainsFactory(info);
        }
    }
}
