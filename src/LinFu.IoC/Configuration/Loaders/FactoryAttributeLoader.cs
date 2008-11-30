using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Injectors;
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

            // The factory must have a default constructor
            // in order to be instantiated
            ConstructorInfo defaultConstructor = sourceType.GetConstructor(new Type[0]);
            if (defaultConstructor == null)
                throw new ArgumentException("The type '{0}' needs a default constructor to be instantiated.", sourceType.AssemblyQualifiedName);

            // Make sure the factory is created only once
            object factoryInstance = Activator.CreateInstance(sourceType, new object[0]);
            if (factoryInstance == null)
                throw new NullReferenceException(string.Format("Unable to create factory type '{0}'",
                                                               sourceType.AssemblyQualifiedName));


            // The factory instance must implement either
            // IFactory or IFactory<T>
            var untypedInstance = factoryInstance as IFactory;
            IEnumerable<Type> factoryInterfaces = (from t in sourceType.GetInterfaces()
                                                   where t.IsGenericType &&
                                                         t.GetGenericTypeDefinition() == typeof(IFactory<>)
                                                   select t);


            if (untypedInstance == null && factoryInterfaces.Count() == 0)
            {
                string message = string.Format("The factory type '{0}' must implement either the IFactory interface or the IFactory<T> interface.", sourceType.AssemblyQualifiedName);
                throw new ArgumentException(message, "sourceType");
            }

            #endregion

            var implementedInterfaces = new HashSet<Type>(factoryInterfaces);

            Func<Type, object, IFactory> createFactory =
                (currentServiceType, factory) =>
                {
                    // Determine if the factory implements
                    // the generic IFactory<T> instance
                    // and use that instance if possible
                    IFactory result = null;
                    Type genericType = typeof(IFactory<>).MakeGenericType(currentServiceType);

                    if (implementedInterfaces.Contains(genericType))
                    {
                        // Convert the IFactory<T> instance down to an IFactory
                        // instance so that it can be used by the target container
                        Type adapterType = typeof(FactoryAdapter<>).MakeGenericType(currentServiceType);
                        result = (IFactory)Activator.CreateInstance(adapterType, new[] { factory });
                        return result;
                    }

                    // Otherwise, use the untyped IFactory instance instead
                    result = factory as IFactory;
                    return result;
                };


            // Build the list of services that this factory can implement
            var servicesToImplement = from f in attributeList
                                      let serviceName = f.ServiceName
                                      let serviceType = f.ServiceType
                                      let factory = createFactory(serviceType, factoryInstance)
                                      where factory != null
                                      select new
                                                 {
                                                     ServiceName = serviceName,
                                                     ServiceType = serviceType,
                                                     FactoryInstance = factory
                                                 };


            var results = new List<Action<IServiceContainer>>();
            foreach (var currentService in servicesToImplement)
            {
                string serviceName = currentService.ServiceName;
                Type serviceType = currentService.ServiceType;
                IFactory factory = currentService.FactoryInstance;

                // HACK: Unnamed custom factories should be able to
                // intercept every request for the given service type
                if (serviceName == null)
                {
                    var injector = new CustomFactoryInjector(serviceType, factory);
                    results.Add(container => container.PreProcessors.Add(injector));
                }
                // Add each service to the container on initialization
                results.Add(container => container.AddFactory(serviceName, serviceType, factory));
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
            return sourceType.IsClass;
        }

        #endregion
    }
}