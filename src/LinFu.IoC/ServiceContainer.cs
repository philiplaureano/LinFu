using System;
using System.Collections.Generic;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a service container with additional
    /// extension points for customizing service instances
    /// </summary>
    public class ServiceContainer : ServiceLocator, IServiceContainer
    {
        private readonly List<IPostProcessor> _postProcessors = new List<IPostProcessor>();
        private readonly List<IPreProcessor> _preprocessors = new List<IPreProcessor>();

        /// <summary>
        /// Initializes the container with the default services.
        /// </summary>
        public ServiceContainer() 
        {
            this.AddDefaultServices();
        }

        protected override IGetService GetDefaultGetServiceBehavior()
        {
            return new DefaultGetServiceBehavior(this);
        }
        protected override IFactoryStorage GetDefaultFactoryStorage()
        {
            return new FactoryStorage();
        }

        protected override IServiceRequest CreateRequest(string serviceName, Type serviceType, object[] additionalArguments, IFactory factory)
        {
            return new ServiceRequest(serviceName, serviceType, additionalArguments, factory, this);
        }

        /// <summary>
        /// Initializes the container with a custom <see cref="ICreateInstance"/> type.
        /// </summary>
        /// <param name="getServiceBehavior">The instance that will be responsible for generating service instances.</param>
        /// <param name="factoryStorage">The <see cref="IFactoryStorage"/> instance responsible for determining which factory instance will instantiate a given service request.</param>
        public ServiceContainer(IGetService getServiceBehavior, IFactoryStorage factoryStorage) : base(getServiceBehavior, factoryStorage)
        {
            this.AddDefaultServices();
        }        

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public virtual void AddFactory(string serviceName, Type serviceType, IEnumerable<Type> additionalParameterTypes,
                                       IFactory factory)
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
            get { return FactoryStorage.AvailableFactories; }
        }
    }
}