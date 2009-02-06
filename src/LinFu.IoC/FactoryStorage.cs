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
