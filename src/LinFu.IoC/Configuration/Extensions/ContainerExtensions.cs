using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Injectors;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Configuration.Resolvers;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC
{
    /// <summary>
    /// A class that adds generics support to existing 
    /// <see cref="IContainer"/> and <see cref="IServiceContainer"/>
    /// instances.
    /// </summary>
    public static class ContainerExtensions
    {
        private static readonly TypeCounter _counter = new TypeCounter();
        private static readonly Stack<Type> _requests = new Stack<Type>();

        /// <summary>
        /// Loads a set of <paramref name="searchPattern">files</paramref> from the <paramref name="directory">target directory</paramref>.
        /// </summary>
        /// <param name="container">The container to be loaded.</param>
        /// <param name="directory">The target directory.</param>
        /// <param name="searchPattern">The search pattern that describes the list of files to be loaded.</param>
        public static void LoadFrom(this IServiceContainer container, string directory,
            string searchPattern)
        {
            var loader = new Loader();

            // Load the target directory
            loader.LoadDirectory(directory, searchPattern);

            // Configure the container
            loader.LoadInto(container);
        }

        /// <summary>
        /// Automatically instantiates a <paramref name="concreteType"/>
        /// with the constructor with the most resolvable parameters from
        /// the given <paramref name="container"/> instance.
        /// </summary>
        /// <param name="container">The service container that contains the arguments that will automatically be injected into the constructor.</param>
        /// <param name="concreteType">The type to instantiate.</param>
        /// <param name="additionalArguments">The list of arguments to pass to the target type.</param>
        /// <returns>A valid, non-null object reference.</returns>
        public static object AutoCreateFrom(this Type concreteType, IServiceContainer container, params object[] additionalArguments)
        {
            return container.AutoCreate(concreteType, additionalArguments);
        }

        /// <summary>
        /// Loads an existing <paramref name="assembly"/> into the container.
        /// </summary>
        /// <param name="container">The target container to be configured.</param>
        /// <param name="assembly">The assembly to be loaded.</param>
        public static void LoadFrom(this IServiceContainer container, Assembly assembly)
        {
            // Use the AssemblyTargetLoader<> class to pull
            // the types out of an assembly
            var assemblyTargetLoader = new AssemblyTargetLoader<IServiceContainer>();
            assemblyTargetLoader.AssemblyActionLoader =
                new AssemblyActionLoader<IServiceContainer>(() => assemblyTargetLoader.TypeLoaders);

            // HACK: Return an existing assembly instead of reading
            // the assembly from disk
            assemblyTargetLoader.AssemblyLoader = new InMemoryAssemblyLoader(assembly);

            // Convert the assembly into a set of configuration actions
            var actions = assemblyTargetLoader.Load(string.Empty).ToList();

            // Apply the actions to the container
            actions.ForEach(action => action(container));
        }

        /// <summary>
        /// Sets the custom attribute type that will be used to mark properties
        /// for automatic injection.
        /// </summary>
        /// <param name="container">The target <see cref="IServiceContainer"/> instance.</param>
        /// <param name="attributeType">The custom property attribute that will be used to mark properties for injection.</param>
        public static void SetCustomPropertyInjectionAttribute(this IServiceContainer container,
            Type attributeType)
        {
            if (attributeType == null)
            {
                // Enable automatic injection for all properties
                container.AddService<IMemberInjectionFilter<PropertyInfo>>(new PropertyInjectionFilter());
                return;
            }

            // Modify the property injection filter to select properties marked
            // with the custom attribute type
            container.AddService<IMemberInjectionFilter<PropertyInfo>>(new AttributedPropertyInjectionFilter(attributeType));
        }

        /// <summary>
        /// Sets the custom attribute type that will be used to mark methods
        /// for automatic injection.
        /// </summary>
        /// <param name="container">The target <see cref="IServiceContainer"/> instance.</param>
        /// <param name="attributeType">The custom property attribute that will be used to mark method for injection.</param>
        public static void SetCustomMethodInjectionAttribute(this IServiceContainer container,
            Type attributeType)
        {
            // Modify the method injection filter to select methods marked
            // with the custom attribute type
            container.AddService<IMemberInjectionFilter<MethodInfo>>(new AttributedMethodInjectionFilter(attributeType));
        }

        /// <summary>
        /// Sets the custom attribute type that will be used to mark fields
        /// for automatic injection.
        /// </summary>
        /// <param name="container">The target <see cref="IServiceContainer"/> instance.</param>
        /// <param name="attributeType">The custom property attribute that will be used to mark fields for injection.</param>
        public static void SetCustomFieldInjectionAttribute(this IServiceContainer container,
            Type attributeType)
        {
            // Modify the method injection filter to select fields marked
            // with the custom attribute type
            container.AddService<IMemberInjectionFilter<FieldInfo>>(new AttributedFieldInjectionFilter(attributeType));
        }

        /// <summary>
        /// Initializes the target <see cref="IServiceContainer"/>
        /// with the default services.
        /// </summary>
        /// <param name="container"></param>
        public static void Initialize(this IServiceContainer container)
        {
            // Load the configuration assembly by default
            container.LoadFrom(typeof(Loader).Assembly);
        }

        /// <summary>
        /// Automatically instantiates a <paramref name="concreteType"/>
        /// with the constructor with the most resolvable parameters from
        /// the given <paramref name="container"/> instance.
        /// </summary>
        /// <param name="container">The service container that contains the arguments that will automatically be injected into the constructor.</param>
        /// <param name="concreteType">The type to instantiate.</param>
        /// <param name="additionalArguments">The list of arguments to pass to the target type.</param>
        /// <returns>A valid, non-null object reference.</returns>
        public static object AutoCreate(this IServiceContainer container, Type concreteType, params object[] additionalArguments)
        {
            // Keep track of the number of pending type requests
            _counter.Increment(concreteType);

            // Keep track of the sequence
            // of requests on the stack
            lock (_requests)
            {
                _requests.Push(concreteType);
            }

            // This is the maximum number of requests per thread per item
            const int maxRequests = 10;

            if (_counter.CountOf(concreteType) > maxRequests)
            {
                // Build the sequence of types that caused the overflow
                var list = new LinkedList<Type>();
                lock (_requests)
                {
                    while (_requests.Count > 0)
                    {
                        var currentType = _requests.Pop();
                        list.AddLast(currentType);
                    }
                }

                throw new RecursiveDependencyException(list);
            }

            // Add the required services if necessary
            container.AddDefaultServices();

            var resolver = container.GetService<IMemberResolver<ConstructorInfo>>();

            // Determine which constructor
            // contains the most resolvable
            // parameters
            var constructor = resolver.ResolveFrom(concreteType, container, additionalArguments);

            var parameterTypes = GetMissingParameterTypes(constructor, additionalArguments);

            // Generate the arguments for the target constructor
            var argumentResolver = container.GetService<IArgumentResolver>();
            var arguments = argumentResolver.ResolveFrom(parameterTypes, container,
                additionalArguments);

            // Instantiate the object
            var constructorInvoke =
                container.GetService<IMethodInvoke<ConstructorInfo>>();

            var result = constructorInvoke.Invoke(null, constructor, arguments);

            lock (_requests)
            {
                _requests.Pop();
            }

            _counter.Decrement(concreteType);

            return result;
        }

        /// <summary>
        /// Initializes the container with the minimum required services.
        /// </summary>
        /// <param name="container">The target service container.</param>
        public static void AddDefaultServices(this IServiceContainer container)
        {
            // Initialize the services only once
            if (container.Contains(typeof(IFactoryBuilder)))
                return;

            container.AddService<IFactoryBuilder>(new FactoryBuilder());

            container.AddService<IMemberResolver<ConstructorInfo>>(new ConstructorResolver(ioc => ioc.GetService<IMethodFinderWithContainer<ConstructorInfo>>()));
            container.AddService<IArgumentResolver>(new ArgumentResolver());

            container.AddService<IMethodInvoke<MethodInfo>>(new MethodInvoke());
            container.AddService<IMethodInvoke<ConstructorInfo>>(new ConstructorInvoke());

            container.AddService<IMethodFinder<ConstructorInfo>>(new MethodFinderFromContainer<ConstructorInfo>());
            container.AddService<IMethodFinderWithContainer<ConstructorInfo>>(new MethodFinderFromContainer<ConstructorInfo>());
            container.AddService<IMethodFinderWithContainer<MethodInfo>>(new MethodFinderFromContainer<MethodInfo>());

            container.AddService<IMethodBuilder<ConstructorInfo>>(new ConstructorMethodBuilder());
            container.AddService<IMethodBuilder<MethodInfo>>(new MethodBuilder());

            container.AddService<IMemberInjectionFilter<MethodInfo>>(new AttributedMethodInjectionFilter());
            container.AddService<IMemberInjectionFilter<FieldInfo>>(new AttributedFieldInjectionFilter());
            container.AddService<IMemberInjectionFilter<PropertyInfo>>(new AttributedPropertyInjectionFilter());

            if (!container.PostProcessors.HasElementWith(p => p is Initializer))
                container.PostProcessors.Add(new Initializer());

            // Add the scope object by default
            container.AddFactory(null, typeof(IScope), new FunctorFactory(f => new Scope()));           
        }

        /// <summary>
        /// Determines which parameter types need to be supplied to invoke a particular
        /// <paramref name="constructor"/>  instance.
        /// </summary>
        /// <param name="constructor">The target constructor.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to invoke the constructor.</param>
        /// <returns>The list of parameter types that are still missing parameter values.</returns>
        private static List<Type> GetMissingParameterTypes(ConstructorInfo constructor,
            ICollection<object> additionalArguments)
        {
            var parameters = from p in constructor.GetParameters()
                             select new { p.Position, Type = p.ParameterType };

            // Determine which parameters need to 
            // be supplied by the container
            var parameterTypes = new List<Type>();
            if (additionalArguments != null && additionalArguments.Count > 0)
            {
                // Supply parameter values for the
                // parameters that weren't supplied by the
                // additionalArguments
                var parameterCount = parameters.Count();
                var maxIndex = parameterCount - additionalArguments.Count;
                var targetParameters = from param in parameters.Where(p => p.Position < maxIndex)
                                       select param.Type;

                parameterTypes.AddRange(targetParameters);
                return parameterTypes;
            }

            var results = from param in parameters
                          select param.Type;

            parameterTypes.AddRange(results);

            return parameterTypes;
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/>
        /// using the given <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="T">The service type to create.</typeparam>
        /// <param name="container">The container that will instantiate the service.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to construct the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public static T GetService<T>(this IServiceContainer container, params object[] additionalArguments)
            where T : class
        {
            Type serviceType = typeof(T);
            return container.GetService(serviceType, additionalArguments) as T;
        }

        /// <summary>
        /// Instantiates a service that matches the <paramref name="info">service description</paramref>.
        /// </summary>
        /// <param name="container">The container that will instantiate the service.</param>
        /// <param name="info">The description of the requested service.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to construct the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public static object GetService(this IServiceContainer container, IServiceInfo info, params object[] additionalArguments)
        {
            return container.GetService(info.ServiceName, info.ServiceType, additionalArguments);
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/>
        /// using the given <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="T">The service type to create.</typeparam>
        /// <param name="container">The container that will instantiate the service.</param>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to construct the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public static T GetService<T>(this IServiceContainer container, string serviceName, params object[] additionalArguments)
            where T : class
        {
            return container.GetService(serviceName, typeof(T), additionalArguments) as T;
        }

        /// <summary>
        /// Configures the container to instantiate the <paramref name="implementingType"/>
        /// on every request for the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="container">The container that will hold the service type.</param>
        /// <param name="serviceType">The type of service being implemented.</param>
        /// <param name="implementingType">The concrete type that will implement the service type.</param>
        public static void AddService(this IServiceContainer container, Type serviceType, Type implementingType)
        {
            container.AddService(null, serviceType, implementingType, LifecycleType.OncePerRequest);
        }

        /// <summary>
        /// Registers the <paramref name="serviceTypeToRegisterAsSelf">service type</paramref>
        /// as both the implementing type and the service type using the given <paramref name="lifecycle"/>.
        /// </summary>
        /// <param name="container">The container that will hold the service type.</param>
        /// <param name="serviceTypeToRegisterAsSelf">The service type that will be registered as both the service type and the implementing type.</param>
        /// <param name="lifecycle">The service <see cref="LifecycleType"/>.</param>
        public static void AddService(this IServiceContainer container, Type serviceTypeToRegisterAsSelf,
            LifecycleType lifecycle)
        {
            container.AddService(serviceTypeToRegisterAsSelf, serviceTypeToRegisterAsSelf, lifecycle);
        }

        /// <summary>
        /// Registers the <paramref name="serviceTypeToRegisterAsSelf">service type</paramref>
        /// as both the implementing type and the service type.
        /// </summary>
        /// <param name="container">The container that will hold the service type.</param>
        /// <param name="serviceTypeToRegisterAsSelf">The service type that will be registered as both the service type and the implementing type.</param>
        public static void AddService(this IServiceContainer container, Type serviceTypeToRegisterAsSelf)
        {
            container.AddService(serviceTypeToRegisterAsSelf, serviceTypeToRegisterAsSelf, LifecycleType.OncePerRequest);
        }

        /// <summary>
        /// Configures the container to instantiate the <paramref name="implementingType"/>
        /// on every request for the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="container">The container that will hold the service type.</param>
        /// <param name="serviceType">The type of service being implemented.</param>
        /// <param name="implementingType">The concrete type that will implement the service type.</param>
        /// <param name="lifecycle">The service <see cref="LifecycleType"/>.</param>
        public static void AddService(this IServiceContainer container, Type serviceType,
            Type implementingType, LifecycleType lifecycle)
        {
            container.AddService(null, serviceType, implementingType, lifecycle);
        }

        /// <summary>
        /// Registers an existing service instance with the container using the given
        /// <paramref name="serviceName"/> and <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="container">The target container instance.</param>
        /// <param name="serviceName">The service name that will be associated with the service instance.</param>
        /// <param name="serviceType">The target service type.</param>
        /// <param name="serviceInstance">The actual service instance that will represent the service type.</param>
        public static void AddService(this IServiceContainer container, string serviceName, Type serviceType, object serviceInstance)
        {
            #region Validation
            if (serviceInstance == null)
                throw new ArgumentNullException("serviceInstance");

            var instanceType = serviceInstance.GetType();
            if (!serviceType.IsAssignableFrom(instanceType))
                throw new ArgumentException(
                    string.Format("The given service instance type '{0}' is not compatible with service type {1}",
                    instanceType.AssemblyQualifiedName,
                    serviceType.AssemblyQualifiedName));
            #endregion

            container.AddFactory(serviceName, serviceType, new InstanceFactory(serviceInstance));
        }

        /// <summary>
        /// Registers an existing service instance with the container using the given
        /// <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="container">The target container instance.</param>
        /// <param name="serviceType">The target service type.</param>
        /// <param name="serviceInstance">The actual service instance that will represent the service type.</param>
        public static void AddService(this IServiceContainer container, Type serviceType, object serviceInstance)
        {
            container.AddService(null, serviceType, serviceInstance);
        }

        /// <summary>
        /// Configures the container to instantiate the <paramref name="implementingType"/>
        /// on every request for the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <paramref name="serviceType"/>.</param>
        /// <param name="container">The container that will hold the service type.</param>
        /// <param name="serviceType">The type of service being implemented.</param>
        /// <param name="implementingType">The concrete type that will implement the service type.</param>
        /// <param name="lifecycle">The service <see cref="LifecycleType"/>.</param>
        public static void AddService(this IServiceContainer container, string serviceName,
            Type serviceType, Type implementingType, LifecycleType lifecycle)
        {
            var factoryBuilder = container.GetService<IFactoryBuilder>();
            var factoryInstance = factoryBuilder.CreateFactory(serviceType, implementingType, lifecycle);

            // Use the standard factory method for non-generic and closed generic types
            if (!serviceType.ContainsGenericParameters && !serviceType.IsAssignableFrom(implementingType))
            {
                var message = string.Format("The implementing type '{0}' must be derived from '{1}'",
                                                implementingType.AssemblyQualifiedName, serviceType.AssemblyQualifiedName);

                throw new ArgumentException(message);
            }

            container.AddFactory(serviceName, serviceType, factoryInstance);
        }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <typeparamref name="T">service type</typeparamref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="container">The container that will hold the factory instance.</param>
        /// <param name="factory">The <see cref="IFactory{T}"/> instance that will create the object instance.</param>
        public static void AddFactory<T>(this IServiceContainer container, string serviceName, IFactory<T> factory)
        {
            IFactory adapter = new FactoryAdapter<T>(factory);
            container.AddFactory(serviceName, typeof(T), adapter);
        }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <typeparamref name="T">service type</typeparamref>.
        /// </summary>        
        /// <param name="container">The container that will hold the factory instance.</param>
        /// <param name="factory">The <see cref="IFactory{T}"/> instance that will create the object instance.</param>
        public static void AddFactory<T>(this IServiceContainer container, IFactory<T> factory)
        {
            IFactory adapter = new FactoryAdapter<T>(factory);
            container.AddFactory(typeof(T), adapter);
        }

        /// <summary>
        /// Registers the <paramref name="factory"/> as the default factory instance
        /// that will be used if no other factory can be found for the current <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="container">The host container.</param>
        /// <param name="serviceType">The service type that will be created by the default factory.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be used if no other factories can create the given service type.</param>
        public static void AddDefaultFactory(this IServiceContainer container, Type serviceType, IFactory factory)
        {
            var injector = new CustomFactoryInjector(serviceType, factory);
            container.PreProcessors.Add(injector);
        }

        /// <summary>
        /// Adds a service to the container by using the given <paramref name="factoryMethod"/> and <paramref name="lifecycleType"/>
        /// to instantiate the service instance.
        /// </summary>
        /// <typeparam name="T">The service type itself.</typeparam>
        /// <param name="serviceName">The name that will be associated with the service instance.</param>
        /// <param name="container">The host container that will instantiate the service type.</param>
        /// <param name="factoryMethod">The factory method that will be used to create the actual service instance.</param>
        /// <param name="lifecycleType">The service <see cref="LifecycleType"/> type.</param>
        public static void AddService<T>(this IServiceContainer container, string serviceName,
            Func<IFactoryRequest, T> factoryMethod, LifecycleType lifecycleType)
        {
            IFactory factory = null;

            // Determine which factory type should be used
            if (lifecycleType == LifecycleType.Singleton)
                factory = new SingletonFactory<T>(factoryMethod);

            if (lifecycleType == LifecycleType.OncePerThread)
                factory = new OncePerThreadFactory<T>(factoryMethod);

            if (lifecycleType == LifecycleType.OncePerRequest)
                factory = new OncePerRequestFactory<T>(factoryMethod);

            container.AddFactory(serviceName, typeof(T), factory);
        }

        /// <summary>
        /// Adds a service to the container by using the given <paramref name="factoryMethod"/> and <paramref name="lifecycleType"/>
        /// to instantiate the service instance.
        /// </summary>
        /// <typeparam name="T">The service type itself.</typeparam>
        /// <param name="container">The host container that will instantiate the service type.</param>
        /// <param name="factoryMethod">The factory method that will be used to create the actual service instance.</param>
        /// <param name="lifecycleType">The service <see cref="LifecycleType"/> type.</param>
        public static void AddService<T>(this IServiceContainer container,
            Func<IFactoryRequest, T> factoryMethod, LifecycleType lifecycleType)
        {
            container.AddService(null, factoryMethod, lifecycleType);
        }

        /// <summary>
        /// Adds an existing service instance to the container.
        /// </summary>
        /// <typeparam name="T">The type of service being added.</typeparam>
        /// <param name="container">The container that will hold the service instance.</param>
        /// <param name="instance">The service instance itself.</param>
        public static void AddService<T>(this IServiceContainer container, T instance)
        {
            container.AddFactory(typeof(T), new InstanceFactory(instance));
        }

        /// <summary>
        /// Adds an existing service instance to the container and
        /// associates it with the <paramref name="serviceName"/>.
        /// </summary>
        /// <typeparam name="T">The type of service being added.</typeparam>
        /// <param name="container">The container that will hold the service instance.</param>
        /// <param name="serviceName">The name that will be associated with the service instance.</param>
        /// <param name="instance">The service instance itself.</param>
        public static void AddService<T>(this IServiceContainer container, string serviceName, T instance)
        {
            container.AddFactory(serviceName, typeof(T), new InstanceFactory(instance));
        }

        /// <summary>
        /// Returns all the services in the container that match the given
        /// <typeparamref name="T">service type</typeparamref>.
        /// </summary>
        /// <typeparam name="T">The type of service to return.</typeparam>        
        /// <param name="container">The target container.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to construct the service type.</param>
        /// <returns>The list of services that implement the given service type.</returns>
        public static IEnumerable<T> GetServices<T>(this IServiceContainer container, params object[] additionalArguments)
        {
            var targetServices = container.AvailableServices.Where(info => info.ServiceType == typeof(T));
            foreach (var info in targetServices)
            {
                yield return (T)container.GetService(info, additionalArguments);
            }
        }

        /// <summary>
        /// Returns a list of services that match the given <paramref name="condition"/>.
        /// </summary>
        /// <param name="condition">The predicate that determines which services should be returned.</param>
        /// <returns>A list of <see cref="IServiceInstance"/> objects that describe the services returned as well as provide a reference to the resulting services themselves.</returns>
        /// <param name="container">the target <see cref="IServiceContainer"/> instance.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to construct the service type.</param>
        public static IEnumerable<IServiceInstance> GetServices(this IServiceContainer container, Func<IServiceInfo, bool> condition, params object[] additionalArguments)
        {
            // Create the services that match
            // the given description
            var results = from info in container.AvailableServices
                          where condition(info) && !info.ServiceType.IsGenericTypeDefinition
                          select
                              new ServiceInstance()
                                  {
                                      ServiceInfo = info,
                                      Object = container.GetService(info, additionalArguments)
                                  } as IServiceInstance;

            return results;
        }

        /// <summary>
        /// Determines whether or not a container contains services that match
        /// the given <paramref name="condition"/>.
        /// </summary>
        /// <param name="container">The target container.</param>
        /// <param name="condition">The predicate that will be used to determine whether or not the requested services exist.</param>
        /// <returns>Returns <c>true</c> if the requested services exist; otherwise, it will return <c>false</c>.</returns>
        public static bool Contains(this IServiceContainer container,
            Func<IServiceInfo, bool> condition)
        {
            var matches = (from info in container.AvailableServices
                           where condition(info)
                           select info).Count();

            return matches > 0;
        }

        /// <summary>
        /// Disables automatic property injection for the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The target container.</param>
        public static void DisableAutoPropertyInjection(this IServiceContainer container)
        {
            container.DisableAutoInjectionFor<PropertyInfo>();
        }

        /// <summary>
        /// Disables automatic method injection for the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The target container.</param>
        public static void DisableAutoMethodInjection(this IServiceContainer container)
        {
            container.DisableAutoInjectionFor<MethodInfo>();
        }

        /// <summary>
        /// Disables automatic field injection for the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The target container.</param>
        public static void DisableAutoFieldInjection(this IServiceContainer container)
        {
            container.DisableAutoInjectionFor<FieldInfo>();
        }

        /// <summary>
        /// Disables automatic dependency injection for members that match the specific
        /// <typeparamref name="TMember"/> type.
        /// </summary>
        /// <typeparam name="TMember">The member injection type to disable.</typeparam>
        /// <param name="container">The target container.</param>
        public static void DisableAutoInjectionFor<TMember>(this IServiceContainer container)
            where TMember : MemberInfo
        {
            // Using the NullMemberInjectionFilter will make sure
            // that no injections will be performed by the target container
            container.AddService<IMemberInjectionFilter<TMember>>(new NullMemberInjectionFilter<TMember>());
        }
    }
}