using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents an <see cref="IFactoryStorage"/> instance that adds generics support to the <see cref="BaseFactoryStorage"/> implementation.
    /// </summary>
    public class FactoryStorage : BaseFactoryStorage
    {
        /// <summary>
        /// Determines whether or not an <see cref="IFactory"/> instance
        /// can be used to create the given service described by the <paramref name="serviceInfo"/> object.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the service to be created.</param>
        /// <returns><c>True</c> if the service can be created; otherwise, it will return <c>false</c>.</returns>
        public override bool ContainsFactory(IServiceInfo serviceInfo)
        {            
            var serviceType = serviceInfo.ServiceType;
            var serviceName = serviceInfo.ServiceName;

            // Use the default implementation for
            // non-generic types
            if (!serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
                return base.ContainsFactory(serviceInfo);

            // If the service type is a generic type, determine
            // if the service type can be created by a 
            // standard factory that can create an instance
            // of that generic type (e.g., IFactory<IGeneric<T>>            
            var result = base.ContainsFactory(serviceInfo);

            // Immediately return a positive match, if possible
            if (result)
                return true;

            if (serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
            {
                // Determine the base type definition
                var baseDefinition = serviceType.GetGenericTypeDefinition();

                // Check if there are any generic factories that can create
                // the entire family of services whose type definitions
                // match the base type
                var genericServiceInfo = new ServiceInfo(serviceName, baseDefinition, serviceInfo.ArgumentTypes);
                result = base.ContainsFactory(genericServiceInfo);
            }

            return result;
        }

        /// <summary>
        /// Obtains the <see cref="IFactory"/> instance that can instantiate the
        /// service described by the <paramref name="serviceInfo"/> object instance.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the service to be created.</param>
        /// <returns>A <see cref="IFactory"/> instance if the service can be instantiated; otherwise, it will return <c>false</c>.</returns>
        public override IFactory GetFactory(IServiceInfo serviceInfo)
        {
            // Attempt to create the service type using
            // the strongly-typed arguments
            var factory = base.GetFactory(serviceInfo);
            var serviceType = serviceInfo.ServiceType;
            var serviceName = serviceInfo.ServiceName;

            // Use the default factory for this service type if no other factory exists
            var defaultServiceInfo = new ServiceInfo(serviceInfo.ServiceName, serviceInfo.ServiceType);
            if (factory == null && base.ContainsFactory(defaultServiceInfo))
                factory = base.GetFactory(defaultServiceInfo);

            // Attempt to create the service type using
            // the generic factories, if possible
            if (factory == null && serviceType.IsGenericType)
            {                
                var definitionType = serviceType.GetGenericTypeDefinition();
                var genericServiceInfo = new ServiceInfo(serviceName, definitionType, serviceInfo.ArgumentTypes);
                factory = base.GetFactory(genericServiceInfo);
            }

            return factory;
        }
    }
}
