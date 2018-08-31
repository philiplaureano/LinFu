using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    ///     Represents the default implementation of the ServiceInfo class.
    /// </summary>
    internal class ServiceInfo : IServiceInfo
    {
        /// <summary>
        ///     Initializes the class with the given <paramref name="serviceName" />
        ///     and <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="serviceType">The type of service that can be created.</param>
        public ServiceInfo(string serviceName, Type serviceType)
        {
            ServiceType = serviceType;
            ServiceName = serviceName;
            ArgumentTypes = new Type[0];
        }

        /// <summary>
        ///     Initializes the class with the given <paramref name="serviceName" />
        ///     and <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="serviceType">The type of service that can be created.</param>
        /// <param name="arguments">The parameter types required by the given service.</param>
        public ServiceInfo(string serviceName, Type serviceType, IEnumerable<Type> arguments)
        {
            ServiceType = serviceType;
            ServiceName = serviceName;
            ArgumentTypes = arguments;
        }


        /// <summary>
        ///     The name of the service being created. By default, this property is blank.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        ///     The type of service being requested.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        ///     Gets a value indicating the list of arguments required by this particular service.
        /// </summary>
        public IEnumerable<Type> ArgumentTypes { get; }


        /// <summary>
        ///     Displays the name of the current service and the current service type.
        /// </summary>
        /// <returns>The name of the current service and the current service type.</returns>
        public override string ToString()
        {
            return string.Format("Service Name: '{0}', Service Type = '{1}'", ServiceName,
                ServiceType.AssemblyQualifiedName);
        }

        /// <summary>
        ///     Determines if the other object is equal to the current <see cref="IServiceInfo" /> instance.
        /// </summary>
        /// <param name="obj">The other object that will be used in the comparison.</param>
        /// <returns>
        ///     Returns <c>true</c> if both instances have the same service name, implement the same service type and have the
        ///     same arguments; otherwise, it will return <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is IServiceInfo))
                return false;

            var other = (IServiceInfo) obj;
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            var hash = 0;

            // Hash the service name
            if (!string.IsNullOrEmpty(ServiceName))
                hash = ServiceName.GetHashCode();

            // Hash the service type
            hash ^= ServiceType.GetHashCode();

            // Hash the arguments
            foreach (var argType in ArgumentTypes)
            {
                if (argType == null)
                    continue;

                hash ^= argType.GetHashCode();
            }

            // Hash the number of arguments
            var argCount = ArgumentTypes == null ? 0 : ArgumentTypes.Count();
            if (argCount > 0)
                hash ^= argCount.GetHashCode();

            return hash;
        }
    }
}