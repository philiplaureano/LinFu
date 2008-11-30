using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Loaders
{
    /// <summary>
    /// A loader class that scans a type for <see cref="ImplementsAttribute"/>
    /// attribute declarations and creates a factory for each corresponding 
    /// attribute instance.
    /// </summary>
    /// <seealso cref="IFactory"/>
    public class ImplementsAttributeLoader : ITypeLoader
    {       
        /// <summary>
        /// Converts a given <see cref="System.Type"/> into
        /// a set of <see cref="Action{IServiceContainer}"/> instances so that
        /// the <see cref="IContainer"/> instance can be loaded
        /// with the given factories.
        /// </summary>
        /// <param name="sourceType">The input type from which one or more factories will be created.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances. This cannot be null.</returns>
        /// 
        public IEnumerable<Action<IServiceContainer>> Load(Type sourceType)
        {
            // Extract the Implements attribute from the source type
            ICustomAttributeProvider provider = sourceType;
            object[] attributes = provider.GetCustomAttributes(typeof(ImplementsAttribute), false);
            List<ImplementsAttribute> attributeList = attributes.Cast<ImplementsAttribute>().ToList();

            var results = new List<Action<IServiceContainer>>();
            IFactory singletonFactory = null;
            foreach (ImplementsAttribute attribute in attributeList)
            {
                string serviceName = attribute.ServiceName;
                Type serviceType = attribute.ServiceType;
                LifecycleType lifeCycle = attribute.LifecycleType;

                IFactory currentFactory = CreateFactory(serviceType, sourceType, lifeCycle);
                if (currentFactory == null)
                    continue;

                // If this type is implemented as a factory singleton,
                // it only needs to be implemented once
                if (lifeCycle == LifecycleType.Singleton)
                {
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
                }

                results.Add(container =>
                            container.AddFactory(serviceName, serviceType, currentFactory));
            }

            return results;
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
        private static IFactory CreateFactory(Type serviceType, Type implementingType, LifecycleType lifecycle)
        {
            // HACK: Use a lazy factory since the actualy IFactoryBuilder instance won't
            // be available until runtime
            Func<IFactoryRequest, object> factoryMethod =
                request =>
                    {
                        var currentContainer = (IServiceContainer)request.Container;
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

                        var actualFactory = builder.CreateFactory(actualServiceType, actualImplementingType, lifecycle);

                        var factoryRequest = new FactoryRequest()
                        {
                            ServiceType = serviceType,
                            ServiceName = request.ServiceName,
                            Arguments = arguments,
                            Container = currentContainer
                        };

                        return actualFactory.CreateInstance(factoryRequest);
                    };

            return new FunctorFactory(factoryMethod);
        }

        /// <summary>
        /// Determines whether or not the current <paramref name="sourceType"/>
        /// can be loaded.
        /// </summary>
        /// <param name="sourceType">The source type currently being loaded.</param>
        /// <returns>Returns <c>true</c> if the type is a class type; otherwise, it returns <c>false</c>.</returns>
        public bool CanLoad(Type sourceType)
        {
            return sourceType.IsClass;
        }        
    }
}