using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IFactoryStorage"/> class.
    /// </summary>
    public class FactoryStorage : IFactoryStorage
    {
        private readonly Dictionary<string, Dictionary<Type, IFactory>> _namedStorage =
            new Dictionary<string, Dictionary<Type, IFactory>>();

        private readonly Dictionary<Type, IFactory> _anonymousStorage = new Dictionary<Type, IFactory>();
        private readonly object _lock = new object();

        /// <summary>
        /// Determines which factories should be used
        /// for a particular service request.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <returns>A factory instance.</returns>
        public IFactory GetFactory(IServiceInfo serviceInfo)
        {
            var serviceName = serviceInfo.ServiceName;
            var serviceType = serviceInfo.ServiceType;

            if (serviceName == null && _anonymousStorage.ContainsKey(serviceType))
                return _anonymousStorage[serviceType];

            if (serviceName != null && _namedStorage.ContainsKey(serviceName) 
                && _namedStorage[serviceName].ContainsKey(serviceType))
                return _namedStorage[serviceName][serviceType];

            return null;
        }

        /// <summary>
        /// Adds a <see cref="IFactory"/> to the current <see cref="IFactoryStorage"/> object.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public void AddFactory(IServiceInfo serviceInfo, IFactory factory)
        {
            var serviceName = serviceInfo.ServiceName;
            var serviceType = serviceInfo.ServiceType;

            lock (_lock)
            {
                Dictionary<Type, IFactory> targetStorage = null;

                if (serviceName == null)
                    targetStorage = _anonymousStorage;

                if (serviceName != null)
                {
                    // If necessary, create a new entry
                    if (!_namedStorage.ContainsKey(serviceName))
                        _namedStorage[serviceName] = new Dictionary<Type, IFactory>();

                    targetStorage = _namedStorage[serviceName];
                }

                // Store the factory
                targetStorage[serviceType] = factory;
            }
        }

        /// <summary>
        /// Determines whether or not a factory exists in storage.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <returns>Returns <c>true</c> if the factory exists; otherwise, it will return <c>false</c>.</returns>
        public bool ContainsFactory(IServiceInfo serviceInfo)
        {
            var serviceName = serviceInfo.ServiceName;
            var serviceType = serviceInfo.ServiceType;

            if (serviceName == null)
                return _anonymousStorage.ContainsKey(serviceType);

            if (_namedStorage.ContainsKey(serviceName) && _namedStorage[serviceName].ContainsKey(serviceType))
                return true;

            return false;
        }

        /// <summary>
        /// Gets a value indicating the list of <see cref="IServiceInfo"/> objects
        /// that describe each available factory in the current <see cref="IFactoryStorage"/>
        /// instance.
        /// </summary>
        public IEnumerable<IServiceInfo> AvailableFactories
        {
            get
            {
                // Add the anonymous services
                var unnamedServices = from serviceType in _anonymousStorage.Keys
                                      where serviceType != null
                                      select new ServiceInfo(null, serviceType) as IServiceInfo;

                // Add the named services
                var namedServices = from serviceName in _namedStorage.Keys
                                    from serviceType in _namedStorage[serviceName].Keys
                                    let serviceInfo = new ServiceInfo(serviceName, serviceType)
                                    select serviceInfo as IServiceInfo;

                var results = new List<IServiceInfo>();
                results.AddRange(unnamedServices);
                results.AddRange(namedServices);

                return results;
            }
        }
    }
}
