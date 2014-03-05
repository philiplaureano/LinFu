using System;
using LinFu.IoC.Factories;
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

            if (!serviceType.IsGenericType || serviceType.IsGenericTypeDefinition)
                return false;

            // Determine the base type definition
            var baseDefinition = serviceType.GetGenericTypeDefinition();

            // Check if there are any generic factories that can create
            // the entire family of services whose type definitions
            // match the base type
            var genericServiceInfo = new ServiceInfo(serviceName, baseDefinition, serviceInfo.ArgumentTypes);
            result = base.ContainsFactory(genericServiceInfo);

            if (result)
                return true;

            if (baseDefinition == typeof (IFactory<>))
            {
                var typeArguments = serviceType.GetGenericArguments();
                var actualServiceType = typeArguments[0];

                var actualServiceInfo = new ServiceInfo(serviceName, actualServiceType, serviceInfo.ArgumentTypes);
                return base.ContainsFactory(actualServiceInfo);
            }

            return false;
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
            factory = GetDefaultFactory(serviceName, serviceType, factory);

            // Attempt to create the service type using
            // the generic factories, if possible
            if (factory != null || !serviceType.IsGenericType)
                return factory;

            var definitionType = serviceType.GetGenericTypeDefinition();
            var genericServiceInfo = new ServiceInfo(serviceName, definitionType, serviceInfo.ArgumentTypes);

            // Find the generic factory that can specifically handle the given argument types
            var containsGenericFactory = base.ContainsFactory(genericServiceInfo);
            if (containsGenericFactory)
                return base.GetFactory(genericServiceInfo);

            // Use the default generic factory if we can't match the given arguments
            var defaultGenericServiceInfo = new ServiceInfo(serviceName, definitionType);
            if (base.ContainsFactory(defaultGenericServiceInfo))
                return base.GetFactory(defaultGenericServiceInfo);


            if (definitionType != typeof (IFactory<>))
                return factory;

            var typeArguments = serviceType.GetGenericArguments();
            var actualServiceType = typeArguments[0];
            factory = GetGenericFactory(serviceInfo, factory, serviceName, actualServiceType);

            return factory;
        }

        /// <summary>
        /// Gets the default factory for a particular service type if no other factory instance can be found.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="factory">The original factory instance that was supposed to be created in order to instantiate the service instance.</param>
        /// <returns>The actual factory instance that will be used to create the service instance.</returns>
        private IFactory GetDefaultFactory(string serviceName, Type serviceType, IFactory factory)
        {
            var defaultNamedServiceInfo = new ServiceInfo(serviceName, serviceType);
            if (factory == null && base.ContainsFactory(defaultNamedServiceInfo))
                factory = base.GetFactory(defaultNamedServiceInfo);

            if (serviceType.IsGenericType)
            {
                var defaultServiceInfo = new ServiceInfo(string.Empty, serviceType);
                if (factory == null && base.ContainsFactory(defaultServiceInfo))
                    factory = base.GetFactory(defaultServiceInfo);
            }


            return factory;
        }

        /// <summary>
        /// Gets the generic factory for a concrete service type.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the service to be created.</param>
        /// <param name="factory">The factory instance that will be used to create the service.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="actualServiceType">The service type.</param>
        /// <returns>A factory instance that can create the generic type.</returns>
        private IFactory GetGenericFactory(IServiceInfo serviceInfo, IFactory factory, string serviceName,
            Type actualServiceType)
        {
            var info = new ServiceInfo(serviceName, actualServiceType, serviceInfo.ArgumentTypes);

            if (base.ContainsFactory(info))
            {
                var actualFactory = base.GetFactory(info);
                Func<IFactoryRequest, object> factoryMethod = request => actualFactory;

                factory = new FunctorFactory<IFactory>(factoryMethod);
            }

            return factory;
        }
    }
}