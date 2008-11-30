using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents the default implementation of the ServiceInfo class.
    /// </summary>
    internal class ServiceInfo : IServiceInfo
    {
        private readonly Type _serviceType;
        private readonly string _serviceName;

        /// <summary>
        /// Initializes the class with the given <paramref name="serviceName"/>
        /// and <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="serviceType">The type of service that can be created.</param>
        public ServiceInfo(string serviceName, Type serviceType)
        {
            _serviceType = serviceType;
            _serviceName = serviceName;
        }

        /// <summary>
        /// The name of the service being created. By default, this property is blank.
        /// </summary>
        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
        }

        /// <summary>
        /// The type of service being requested.
        /// </summary>
        public Type ServiceType
        {
            get
            {
                return _serviceType;
            }
        }
    }
}
