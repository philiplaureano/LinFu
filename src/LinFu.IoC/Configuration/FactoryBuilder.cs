using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Configuration.Loaders;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// The default implementation of the <see cref="IFactoryBuilder"/> class.
    /// </summary>
    internal class FactoryBuilder : IFactoryBuilder
    {
        private static readonly Dictionary<LifecycleType, Type> _factoryTypes = new Dictionary<LifecycleType, Type>();
        private static readonly IServiceContainer _dummyContainer = new ServiceContainer();

        /// <summary>
        /// Initializes the list of factory types.
        /// </summary>
        static FactoryBuilder()
        {
            _factoryTypes[LifecycleType.OncePerRequest] = typeof(OncePerRequestFactory<>);
            _factoryTypes[LifecycleType.OncePerThread] = typeof(OncePerThreadFactory<>);
            _factoryTypes[LifecycleType.Singleton] = typeof(SingletonFactory<>);
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
        public IFactory CreateFactory(Type serviceType, Type implementingType, LifecycleType lifecycle)
        {
            // Determine the factory type
            Type factoryTypeDefinition = _factoryTypes[lifecycle];


            Type actualType = GetActualType(serviceType, implementingType);

            if (!serviceType.ContainsGenericParameters && !actualType.ContainsGenericParameters)
            {
                Type factoryType = factoryTypeDefinition.MakeGenericType(serviceType);
                return CreateFactory(serviceType, actualType, factoryType);
            }

            Func<IFactoryRequest, object> factoryMethod =
                request =>
                {
                    var serviceName = request.ServiceName;
                    var type = request.ServiceType;
                    var currentContainer = request.Container;
                    var arguments = request.Arguments;

                    // Determine the implementing type
                    var concreteType = GetActualType(type, implementingType);

                    // The concrete type cannot be null
                    if (concreteType == null)
                        return null;

                    // Generate the concrete factory instance 
                    // at runtime
                    Type factoryType = factoryTypeDefinition.MakeGenericType(type);
                    var factory = CreateFactory(type, concreteType, factoryType);

                    var factoryRequest = new FactoryRequest()
                    {
                        ServiceType = serviceType,
                        ServiceName = serviceName,
                        Arguments = arguments,
                        Container = currentContainer
                    };

                    return factory.CreateInstance(factoryRequest);
                };

            return new FunctorFactory(factoryMethod);
        }

        /// <summary>
        /// Creates a factory instance that can create instaces of the given
        /// <paramref name="serviceType"/>  using the <paramref name="actualType"/>
        /// as the implementation.
        /// </summary>
        /// <param name="serviceType">The service being implemented.</param>
        /// <param name="actualType">The actual type that will implement the service.</param>
        /// <param name="factoryType">The factory type that will instantiate the target service.</param>
        /// <returns>A valid <see cref="IFactory"/> instance.</returns>
        private IFactory CreateFactory(Type serviceType, Type actualType, Type factoryType)
        {
            // Create the factory itself
            MulticastDelegate factoryMethod = CreateFactoryMethod(serviceType, actualType);
            
            object factoryInstance = factoryType.AutoCreateFrom(_dummyContainer, factoryMethod);
            var result = factoryInstance as IFactory;

            return result;
        }

        /// <summary>
        /// Determines the implementing concrete type from the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="implementingType">The concrete class that will implement the service type.</param>
        /// <returns>The actual implementing type.</returns>
        private static Type GetActualType(Type serviceType, Type implementingType)
        {
            if (!implementingType.ContainsGenericParameters)
                return implementingType;

            var actualType = implementingType;

            // The service type must be a generic type with
            // closed generic parameters
            if (!serviceType.IsGenericType || serviceType.ContainsGenericParameters)
                return implementingType;


            // Attempt to apply the generic parameters of the service type
            // to the implementing type
            var typeParameters = serviceType.GetGenericArguments();
            try
            {
                var concreteType = implementingType.MakeGenericType(typeParameters);

                // The concrete type must derive from the given service type
                if (serviceType.IsAssignableFrom(concreteType))
                    actualType = concreteType;
            }
            catch
            {
                // Ignore the error
            }

            return actualType;
        }

        /// <summary>
        /// A <c>private</c> method that creates the factory method delegate
        /// for use with a particular factory class.
        /// </summary>
        /// <seealso cref="SingletonFactory{T}"/>
        /// <seealso cref="OncePerRequestFactory{T}"/>
        /// <seealso cref="OncePerThreadFactory{T}"/>
        /// <param name="serviceType">The service type being instantiated.</param>
        /// <param name="implementingType">The type that will provide the implementation for the actual service.</param>
        /// <returns>A factory method delegate that can create the given service.</returns>
        private MulticastDelegate CreateFactoryMethod(Type serviceType, Type implementingType)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

            MethodInfo factoryMethodDefinition = typeof(FactoryBuilder).GetMethod("CreateFactoryMethodInternal", flags);
            MethodInfo factoryMethod = factoryMethodDefinition.MakeGenericMethod(serviceType, implementingType);

            // Create the Func<IFactoryRequest, TService> factory delegate
            var result = factoryMethod.Invoke(null, new object[0]) as MulticastDelegate;

            return result;
        }

        /// <summary>
        /// A method that generates the actual lambda function that creates
        /// the new service instance.
        /// </summary>
        /// <typeparam name="TService">The service type being instantiated.</typeparam>
        /// <typeparam name="TImplementation">The type that will provide the implementation for the actual service.</typeparam>
        /// <returns>A strongly-typed factory method delegate that can create the given service.</returns>
        internal static Func<IFactoryRequest, TService> CreateFactoryMethodInternal<TService, TImplementation>()
            where TImplementation : TService
        {
            return request =>
            {
                var container = request.Container;
                var arguments = request.Arguments;
                var serviceContainer = (IServiceContainer)container;

                // Attempt to autoresolve the constructor
                if (serviceContainer != null)
                    return (TService)serviceContainer.AutoCreate(typeof(TImplementation), arguments);

                // Otherwise, use the default constructor
                return (TService)Activator.CreateInstance(typeof(TImplementation), arguments);
            };
        }
    }
}
