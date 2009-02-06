using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a service container with additional
    /// extension points for customizing service instances
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        private readonly IFactoryStorage _factoryStorage = new FactoryStorage();
        private readonly IGetService _getServiceBehavior;
        private readonly List<IPostProcessor> _postProcessors = new List<IPostProcessor>();
        private readonly List<IPreProcessor> _preprocessors = new List<IPreProcessor>();

        /// <summary>
        /// Initializes the container with the default services.
        /// </summary>
        public ServiceContainer()
        {
            _getServiceBehavior = new DefaultGetServiceBehavior(this);
            this.AddDefaultServices();
        }

        /// <summary>
        /// Initializes the container with a custom <see cref="ICreateInstance"/> type.
        /// </summary>
        /// <param name="getServiceBehavior">The instance that will be responsible for generating service instances.</param>
        /// <param name="factoryStorage">The <see cref="IFactoryStorage"/> instance responsible for determining which factory instance will instantiate a given service request.</param>
        public ServiceContainer(IGetService getServiceBehavior, IFactoryStorage factoryStorage)
        {
            if (getServiceBehavior == null)
                throw new ArgumentNullException("getServiceBehavior");

            if (factoryStorage == null)
                throw new ArgumentNullException("factoryStorage");

            _getServiceBehavior = getServiceBehavior;
            _factoryStorage = factoryStorage;

            this.AddDefaultServices();
        }

        /// <summary>
        /// Gets or sets a <see cref="bool">System.Boolean</see> value
        /// that determines whether or not the container should throw
        /// a <see cref="ServiceNotFoundException"/> if a requested service
        /// cannot be found or created.
        /// </summary>
        public virtual bool SuppressErrors { get; set; }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public virtual void AddFactory(string serviceName, Type serviceType, IEnumerable<Type> additionalParameterTypes, IFactory factory)
        {
            FactoryStorage.AddFactory(serviceName, serviceType, additionalParameterTypes, factory);
        }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The service type to associate with the factory</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be responsible for creating the service instance</param>
        public virtual void AddFactory(Type serviceType, IEnumerable<Type> additionalParameterTypes, IFactory factory)
        {
            AddFactory(null, serviceType, additionalParameterTypes, factory);
        }

        /// <summary>
        /// Determines whether or not the given <paramref name="serviceType"/>
        /// can be instantiated by the container.
        /// </summary>
        /// <param name="serviceType">The type of service to instantiate.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool Contains(Type serviceType, IEnumerable<Type> additionalParameterTypes)
        {
            return Contains(null, serviceType, additionalParameterTypes);
        }

        /// <summary>
        /// Overridden. Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <remarks>
        /// This overload of the <c>GetService</c> method has been overridden
        /// so that its results can be handled by the postprocessors.
        /// </remarks>
        /// <seealso cref="IPostProcessor"/>
        /// <param name="serviceType">The service type to instantiate.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a null value.</returns>
        public object GetService(Type serviceType, params object[] additionalArguments)
        {
            return GetService(null, serviceType, additionalArguments);
        }

        /// <summary>
        /// Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The service type to instantiate.</param>        
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public virtual object GetService(string serviceName, Type serviceType, params object[] additionalArguments)
        {
            IFactory factory = null;

            if (FactoryStorage != null)
                factory = FactoryStorage.GetFactory(serviceName, serviceType, additionalArguments);

            var serviceRequest = new ServiceRequest(serviceName, serviceType, additionalArguments, factory, this);
            var instance = _getServiceBehavior.GetService(serviceRequest);

            if (SuppressErrors == false && instance == null && serviceName == null)
                throw new ServiceNotFoundException(serviceType);

            if (SuppressErrors == false && instance == null && serviceName != null)
                throw new NamedServiceNotFoundException(serviceName, serviceType);

            return instance;
        }

        /// <summary>
        /// Determines whether or not a service can be created using
        /// the given <paramref name="serviceName">service name</paramref>
        /// and <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool Contains(string serviceName, Type serviceType, IEnumerable<Type> additionalParameterTypes)
        {
            return FactoryStorage.ContainsFactory(serviceName, serviceType, additionalParameterTypes);
        }

        /// <summary>
        /// Gets the value indicating the <see cref="IFactoryStorage"/> instance
        /// that will be used to store each <see cref="IFactory"/> instance.
        /// </summary>
        internal IFactoryStorage FactoryStorage
        {
            get
            {
                return _factoryStorage;
            }
        }

        /// <summary>
        /// The list of postprocessors that will handle every
        /// service request result.
        /// </summary>
        public IList<IPostProcessor> PostProcessors
        {
            get { return _postProcessors; }
        }

        /// <summary>
        /// The list of preprocessors that will handle
        /// every service request before each actual service is created.
        /// </summary>
        public IList<IPreProcessor> PreProcessors
        {
            get { return _preprocessors; }
        }

        /// <summary>
        /// The list of services currently available inside the container.
        /// </summary>
        public virtual IEnumerable<IServiceInfo> AvailableServices
        {
            get
            {
                return FactoryStorage.AvailableFactories;
            }
        }
    }
}
