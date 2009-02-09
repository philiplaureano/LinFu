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
        private readonly IEnumerable<Type> _arguments;
        
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
            _arguments = new Type[0];
        }

        /// <summary>
        /// Initializes the class with the given <paramref name="serviceName"/>
        /// and <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="serviceType">The type of service that can be created.</param>
        /// <param name="arguments">The parameter types required by the given service.</param>
        public ServiceInfo(string serviceName, Type serviceType, IEnumerable<Type> arguments)
        {
            _serviceType = serviceType;
            _serviceName = serviceName;
            _arguments = arguments;
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

        /// <summary>
        /// Gets a value indicating the list of arguments required by this particular service.
        /// </summary>
        public IEnumerable<Type> ArgumentTypes
        {
            get { return _arguments; }
        }

        /// <summary>
        /// <summary>
        /// Displays the name of the current service and the current service type.
        /// </summary>
        /// <returns>The name of the current service and the current service type.</returns>
        public override string ToString()
        {
            return string.Format("Service Name: '{0}', Service Type = '{1}'", ServiceName,
                                 ServiceType.AssemblyQualifiedName);
        }
        /// Determines if the other object is equal to the current <see cref="IServiceInfo"/> instance.
        /// </summary>
        /// <param name="obj">The other object that will be used in the comparison.</param>
        /// <returns>Returns <c>true</c> if both instances have the same service name, implement the same service type and have the same arguments; otherwise, it will return <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is IServiceInfo))
                return false;

            var other = (IServiceInfo) obj;
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hash = 0;

            // Hash the service name
            if (!string.IsNullOrEmpty(_serviceName))
                hash = _serviceName.GetHashCode();

            // Hash the service type
            hash ^= _serviceType.GetHashCode();

            // Hash the arguments
            foreach(var argType in _arguments)
            {
                if (argType == null)
                    continue;

                hash ^= argType.GetHashCode();
            }

            // Hash the number of arguments
            var argCount = _arguments == null ? 0 : _arguments.Count();
            if (argCount > 0)
                hash ^= argCount.GetHashCode();

            return hash;
        }
    }
}
