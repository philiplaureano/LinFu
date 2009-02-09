using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Injectors;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that injects custom <see cref="IFactory"/> and <see cref="IFactory{T}"/>
    /// instances into an <see cref="IServiceContainer"/> instance.
    /// </summary>
    public class FactoryAttributeLoader : ITypeLoader
    {
        #region ITypeLoader Members

        /// <summary>
        /// Loads an <see cref="IFactory"/> and <see cref="IFactory{T}"/> instance
        /// into a <see cref="IServiceContainer"/> instance using the given
        /// <paramref name="sourceType"/>.
        /// </summary>
        /// <param name="sourceType">The input type from which one or more factories will be created.</param>
        /// <returns>A set of <see cref="Action{T}"/> instances. This cannot be null.</returns>
        public IEnumerable<Action<IServiceContainer>> Load(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            // Extract the factory attributes from the current type
            object[] attributes = sourceType.GetCustomAttributes(typeof(FactoryAttribute), false);
            List<FactoryAttribute> attributeList = attributes.Cast<FactoryAttribute>()
                .Where(f => f != null).ToList();

            #region Validation

            // The target type must have at least one
            // factory attribute
            if (attributeList.Count == 0)
                return new Action<IServiceContainer>[0];


            // Make sure the factory is created only once            
            Func<IFactoryRequest, object> getFactoryInstance = request =>
            {
                var container = request.Container;
                return container.AutoCreateInternal(sourceType);
            };

            #endregion

            return GetResults(sourceType, attributeList, getFactoryInstance);
        }

        /// <summary>
        /// Instantiates the <see cref="IFactory"/> instances associated with the <paramref name="sourceType"/> and
        /// adds those factories to the target container upon initialization.
        /// </summary>
        /// <param name="sourceType">The <see cref="System.Type"/> currently being inspected.</param>
        /// <param name="attributeList">The list of <see cref="FactoryAttribute"/> instances currently declared on on the source type.</param>
        /// <param name="getFactoryInstance">The functor that will be responsible for generating the factory instance.</param>
        /// <returns>A list of actions that will add the factories to the target container.</returns>
        private static IEnumerable<Action<IServiceContainer>> GetResults(Type sourceType, IEnumerable<FactoryAttribute> attributeList,
             Func<IFactoryRequest, object> getFactoryInstance)
        {
            var results = new List<Action<IServiceContainer>>();
            // The factory instance must implement either
            // IFactory or IFactory<T>
            IEnumerable<Type> factoryInterfaces = (from t in sourceType.GetInterfaces()
                                                   where t.IsGenericType &&
                                                         t.GetGenericTypeDefinition() == typeof(IFactory<>)
                                                   select t);

            if (!typeof(IFactory).IsAssignableFrom(sourceType) && factoryInterfaces.Count() == 0)
            {
                string message = string.Format("The factory type '{0}' must implement either the IFactory interface or the IFactory<T> interface.", sourceType.AssemblyQualifiedName);
                throw new ArgumentException(message, "sourceType");
            }

            var implementedInterfaces = new HashSet<Type>(factoryInterfaces);
            Func<Type, Func<IFactoryRequest, object>, IFactory> createFactory =
                (currentServiceType, getFactory) =>
                {
                    // Determine if the factory implements
                    // the generic IFactory<T> instance
                    // and use that instance if possible

                    Func<IFactoryRequest, IFactory> getStronglyTypedFactory =
                        request =>
                        {
                            object result = getFactory(request);
                            // If the object is IFactory then we can just return it.
                            if (result is IFactory)                                
                                return (IFactory)result;

                            // Check to see if the object is IFactory<T>, if so we need to adapt it to 
                            // IFactory.
                            Type genericType = typeof(IFactory<>).MakeGenericType(currentServiceType);
                            if (!genericType.IsInstanceOfType(result))
                            {
                                // It isn't an IFactory or IFactory<T>, who knows what it is, return null.
                                return null;
                            }

                            // Adapt IFactory<T> to IFactory.
                            var adapterType = typeof(FactoryAdapter<>).MakeGenericType(currentServiceType);
                            var adapter = (IFactory)Activator.CreateInstance(adapterType, new[] { result });
                            return adapter;

                        };

                    return GetFactory(currentServiceType, getStronglyTypedFactory, implementedInterfaces);
                };

            // Build the list of services that this factory can implement
            var servicesToImplement = from f in attributeList
                                      let serviceName = f.ServiceName
                                      let serviceType = f.ServiceType
                                      let argumentTypes = f.ArgumentTypes ?? new Type[0]
                                      let factory = createFactory(serviceType, getFactoryInstance)
                                      where factory != null
                                      select new
                                                 {
                                                     ServiceName = serviceName,
                                                     ServiceType = serviceType,
                                                     ArgumentTypes = argumentTypes,
                                                     FactoryInstance = factory
                                                 };


            foreach (var currentService in servicesToImplement)
            {
                var serviceName = currentService.ServiceName;
                var serviceType = currentService.ServiceType;
                var argumentTypes = currentService.ArgumentTypes;
                var factory = currentService.FactoryInstance;

                // HACK: Unnamed custom factories should be able to
                // intercept every request for the given service type
                if (serviceName == null)
                {
                    var injector = new CustomFactoryInjector(serviceType, factory);
                    results.Add(container => container.PreProcessors.Add(injector));
                }
                // Add each service to the container on initialization
                results.Add(container => container.AddFactory(serviceName, serviceType, argumentTypes, factory));
            }

            return results;
        }

        /// <summary>
        /// Instantiates the given factory using the <paramref name="getStronglyTypedFactory">factory functor.</paramref>
        /// </summary>
        /// <param name="currentServiceType">The service type that will be created by the factory.</param>
        /// <param name="getStronglyTypedFactory">The functor that will be responsible for creating the factory itself.</param>
        /// <param name="implementedInterfaces">The list of <see cref="IFactory{T}"/> interfaces that are implemented by the source type.</param>
        /// <returns>A valid factory instance.</returns>
        private static IFactory GetFactory(Type currentServiceType, Func<IFactoryRequest, IFactory> getStronglyTypedFactory,
            ICollection<Type> implementedInterfaces)
        {
            Type genericType = typeof(IFactory<>).MakeGenericType(currentServiceType);

            // Lazy-instantiate the factories so that they can be injected by the container
            var lazyFactory = new LazyFactory(getStronglyTypedFactory);

            IFactory result;
            if (implementedInterfaces.Contains(genericType))
            {
                // Convert the IFactory<T> instance down to an IFactory
                // instance so that it can be used by the target container                        
                Type adapterType = typeof(FactoryAdapter<>).MakeGenericType(currentServiceType);
                result = (IFactory)Activator.CreateInstance(adapterType, new[] { lazyFactory });
                return result;
            }

            // Otherwise, use the untyped IFactory instance instead
            result = lazyFactory;
            return result;
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

        #endregion
    }
}