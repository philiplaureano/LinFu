using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinFu.Finders;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IArgumentResolver"/> class.
    /// </summary>
    public class ArgumentResolver : IArgumentResolver
    {
        #region IArgumentResolver Members

        /// <summary>
        /// Generates method arguments from the given <paramref name="parameterTypes"/>
        /// and <paramref name="container"/>.
        /// </summary>
        /// <param name="parameterTypes">The parameter types for the target method.</param>
        /// <param name="container">The container that will provide the method arguments.</param>
        /// <param name="additionalArguments">The additional arguments that will be passed to the target method.</param>
        /// <returns>An array of objects that represent the arguments to be passed to the target method.</returns>
        public object[] ResolveFrom(IEnumerable<INamedType> parameterTypes, IServiceContainer container,
                                    params object[] additionalArguments)
        {
            var enumerableDefinition = typeof (IEnumerable<>);
            var factoryDefinition = typeof (IFactory<>);
            var argumentList = new List<object>();
            foreach (var namedType in parameterTypes)
            {
                var parameterType = namedType.Type;

                // Use the named service instance if possible
                var parameterName = namedType.Name;
                string serviceName = null;

                if (!string.IsNullOrEmpty(parameterName) && parameterName.Length > 1)
                {
                    var firstChar = parameterName.First().ToString();
                    var remainingText = parameterName.Substring(1);
                    serviceName = string.Format("{0}{1}", firstChar.ToUpper(), remainingText);
                }

                if (serviceName != null && container.Contains(serviceName, parameterType))
                {
                    // Instantiate the service type and build
                    // the argument list
                    var currentArgument = container.GetService(serviceName, parameterType);
                    argumentList.Add(currentArgument);
                    continue;
                }

                // Substitute the parameter if and only if 
                // the container does not have service that
                // that matches the parameter type
                var parameterTypeExists = container.Contains(parameterType);
                if (parameterTypeExists)
                {
                    // Instantiate the service type and build
                    // the argument list
                    var currentArgument = container.GetService(parameterType);
                    argumentList.Add(currentArgument);
                    continue;
                }

                // Determine if the parameter type is an IEnumerable<T> type
                // and generate the list if necessary
                if (parameterType.IsGenericType &&
                    parameterType.GetGenericTypeDefinition() == enumerableDefinition)
                {
                    AddEnumerableArgument(parameterType, container, argumentList);
                    continue;
                }

                if (parameterType.IsArray)
                {
                    // Determine if the parameter type is an array
                    // of existing services and inject the current
                    // set of services as a parameter value
                    AddArrayArgument(parameterType, container, argumentList);
                }
            }

            // Append the existing arguments
            if (additionalArguments != null && additionalArguments.Length > 0)
                argumentList.AddRange(additionalArguments);

            return argumentList.ToArray();
        }

        #endregion

        /// <summary>
        /// Constructs an array of services using the services currently available
        /// in the <paramref name="container"/>.
        /// </summary>
        /// <param name="parameterType">The current parameter type.</param>
        /// <param name="container">The container that will be used to build the array of services.</param>
        /// <param name="argumentList">The list that will store new service array.</param>
        private static void AddArrayArgument(Type parameterType, IServiceContainer container,
                                             ICollection<object> argumentList)
        {
            var isArrayOfServices = parameterType.ExistsAsServiceArray();
            if (!isArrayOfServices(container))
                return;

            var elementType = parameterType.GetElementType();

            // Instantiate all services that match
            // the element type
            var services = (from info in container.AvailableServices
                                            where info.ServiceType == elementType
                                            select container.GetService(info));

            var serviceArray = services.Cast(elementType);
            argumentList.Add(serviceArray);
        }

        /// <summary>
        /// Determines whether or not a parameter type is an existing
        /// list of available services and automatically constructs the
        /// service list and adds it to the <paramref name="argumentList"/>.
        /// </summary>
        /// <param name="parameterType">The current constructor parameter type.</param>
        /// <param name="container">The container that will provide the argument values.</param>
        /// <param name="argumentList">The list that will hold the arguments to be passed to the constructor.</param>
        private static void AddEnumerableArgument(Type parameterType, IServiceContainer container,
                                                  ICollection<object> argumentList)
        {
            var elementType = parameterType.GetGenericArguments()[0];
            var baseElementDefinition = elementType.IsGenericType
                                             ? elementType.GetGenericTypeDefinition()
                                             : null;

            // There has to be at least one service
            Func<IServiceInfo, bool> condition =
                info => info.ServiceType == elementType;

            // If the element is a generic type,
            // we need to check for any available generic factory
            // instances that might be able to create the element type
            if (baseElementDefinition != null)
                condition = condition.Or(info => info.ServiceType == baseElementDefinition);

            if (!container.Contains(condition))
                return;

            var serviceList = new List<object>();

            // Build the IEnumerable<> list of services
            // that match the gvien condition
            var services = container.GetServices(condition);
            foreach (var service in services)
            {
                serviceList.Add(service.Object);
            }

            IEnumerable enumerable = serviceList.AsEnumerable();

            var result = enumerable.Cast(elementType);
            argumentList.Add(result);
        }
    }
}