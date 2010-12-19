using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Loaders
{
    public abstract class ServiceLoader : ITypeLoader
    {
        /// <summary>
        /// Converts a given <see cref="System.Type"/> into
        /// a set of <see cref="Action{IServiceContainer}"/> instances so that
        /// the <see cref="IContainer"/> instance can be loaded
        /// with the given factories.
        /// </summary>
        /// <param name="sourceType">The input type from which one or more factories will be created.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances. This cannot be null.</returns>
        public IEnumerable<Action<IServiceContainer>> Load(Type sourceType)
        {
            var implementations = GetImplementations(sourceType);
            return GetTypeLoadActions(sourceType, implementations);
        }

        /// <summary>
        /// Gets the <see cref="IImplementationInfo"/> instances that describe the services that the given <paramref name="sourceType"/>
        /// can implement.
        /// </summary>
        /// <param name="sourceType">The source type that represents the implementing type.</param>
        /// <returns>A list of <see cref="IImplementationInfo"/> object instances that can be implemented by the given <paramref name="sourceType"/></returns>
        protected abstract IEnumerable<IImplementationInfo> GetImplementations(Type sourceType);

        /// <summary>
        /// Gets the actions that will be used to instantiate the given <paramref name="sourceType"/> and its <paramref name="implementations"/>.
        /// </summary>
        /// <param name="sourceType">The source type that represents the implementing type.</param>
        /// <param name="implementations">The implementations that the given source type can create.</param>
        /// <returns></returns>
        private IEnumerable<Action<IServiceContainer>> GetTypeLoadActions(Type sourceType, IEnumerable<IImplementationInfo> implementations)
        {
            var results = new List<Action<IServiceContainer>>();
            IFactory singletonFactory = null;
            foreach (var attribute in implementations)
            {
                var serviceName = attribute.ServiceName;
                var serviceType = attribute.ServiceType;
                var lifeCycle = attribute.LifecycleType;

                var currentFactory = CreateFactory(serviceType, sourceType, lifeCycle);
                if (currentFactory == null)
                    continue;

                // If this type is implemented as a factory singleton,
                // it only needs to be implemented once
                if (lifeCycle != LifecycleType.Singleton)
                {
                    results.Add(container =>
                                container.AddFactory(serviceName, serviceType, currentFactory));
                    continue;
                }

                if (singletonFactory == null)
                {
                    // Initialize the singleton instance only once
                    singletonFactory = currentFactory;
                }
                else
                {
                    // Make sure that the same singleton factory instance
                    // is assigned to every single point
                    // where it is marked as a singleton
                    currentFactory = singletonFactory;
                }
                results.Add(container =>
                            container.AddFactory(serviceName, serviceType, currentFactory));
                continue;
            }

            return results;
        }

        /// <summary>
        /// Determines whether or not the current <paramref name="sourceType"/>
        /// can be loaded.
        /// </summary>
        /// <param name="sourceType">The source type currently being loaded.</param>
        /// <returns>Returns <c>true</c> if the type is a class type; otherwise, it returns <c>false</c>.</returns>
        public bool CanLoad(Type sourceType)
        {
            try
            {
                return sourceType.IsClass;
            }
            catch (TypeInitializationException)
            {
                // Ignore the error
                return false;
            }
            catch (FileNotFoundException)
            {
                // Ignore the error
                return false;
            }
        }

        /// <summary>
        /// Creates a factory instance that can create instaces of the given
        /// <paramref name="serviceType"/>  using the <paramref name="implementingType"/>
        /// as the implementation.
        /// </summary>
        /// <param name="serviceType">The service being implemented.</param>
        /// <param name="implementingType">The actual type that will implement the service.</param>
        /// <param name="lifecycle">The <see cref="LifecycleType"/> that determines the lifetime of each instance being created.</param>
        /// <returns>A valid <see cref="IFactory"/> instance.</returns>
        private IFactory CreateFactory(Type serviceType, Type implementingType, LifecycleType lifecycle)
        {
            // HACK: Use a lazy factory since the actualy IFactoryBuilder instance won't
            // be available until runtime
            Func<IFactoryRequest, object> factoryMethod =
                request => CreateInstance(request, serviceType, implementingType, lifecycle);

            return new FunctorFactory(factoryMethod);
        }

        /// <summary>
        /// Creates the service instance with the current set of parameters.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the service request.</param>
        /// <param name="serviceType">The service type that needs to be instantiated.</param>
        /// <param name="implementingType">The implementing type that will be used to instantiate the service.</param>
        /// <param name="lifecycle">The instancing type for the given factory request.</param>
        /// <returns>A service instance.</returns>
        private object CreateInstance(IFactoryRequest request, Type serviceType, Type implementingType, LifecycleType lifecycle)
        {
            var currentContainer = request.Container;
            var arguments = request.Arguments;
            var builder = currentContainer.GetService<IFactoryBuilder>();

            // HACK: If the service type is a type definition and
            // the implementing type is a type definition,
            // assume that the service type has the same number of
            // generic arguments as the implementing type
            var actualServiceType = serviceType;
            var actualImplementingType = implementingType;
            if (serviceType.IsGenericTypeDefinition && implementingType.IsGenericTypeDefinition &&
                serviceType.GetGenericArguments().Count() == implementingType.GetGenericArguments().Count())
            {
                var typeArguments = request.ServiceType.GetGenericArguments();
                actualServiceType = serviceType.MakeGenericType(typeArguments);
                actualImplementingType = implementingType.MakeGenericType(typeArguments);
            }

            IFactory actualFactory = builder.CreateFactory(actualServiceType, actualImplementingType,
                                                           lifecycle);

            var factoryRequest = new FactoryRequest
                                     {
                                         ServiceType = serviceType,
                                         ServiceName = request.ServiceName,
                                         Arguments = arguments,
                                         Container = currentContainer
                                     };

            return actualFactory.CreateInstance(factoryRequest);
        }
    }
}