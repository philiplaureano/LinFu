using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.Finders;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
using System.Collections;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Adds methods that extend LinFu.IoC to support automatic constructor resolution.
    /// </summary>
    public static class ResolutionExtensions
    {
        /// <summary>
        /// Generates a predicate that determines whether or not a specific parameter type
        /// exists in a container.
        /// </summary>
        /// <param name="parameterType">The target <see cref="Type"/>. </param>
        /// <returns>A a predicate that determines whether or not a specific type
        /// exists in a container</returns>
        public static Func<IServiceContainer, bool> MustExistInContainer(this Type parameterType)
        {
            return container => container.Contains(parameterType);
        }
        /// <summary>
        /// Generates a predicate that determines whether or not a specific type is actually
        /// a list of services that can be created from a given container.
        /// </summary>
        /// <param name="parameterType">The target <see cref="Type"/>. </param>
        /// <returns>A a predicate that determines whether or not a specific type
        /// exists as a list of services in a container</returns>
        public static Func<IServiceContainer, bool> ExistsAsEnumerableSetOfServices(this Type parameterType)
        {
            // The type must be derived from IEnumerable<T>            
            var enumerableDefinition = typeof(IEnumerable<>);

            if (!parameterType.IsGenericType || parameterType.GetGenericTypeDefinition() != enumerableDefinition)
                return container => false;

            // Determine the individual service type
            var elementType = parameterType.GetGenericArguments()[0];
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            // If this type isn't an IEnumerable<T> type, there's no point in testing
            // if it is a list of services that exists in the container
            if (!enumerableType.IsAssignableFrom(parameterType))
                return container => false;

            // A single service instance implies that a list of services can be created
            // from the current container
            Func<IServiceContainer, bool> hasService = container => container.Contains(elementType);

            // Check for any named services that can be included in the service list
            Func<IServiceContainer, bool> hasNamedService = container =>
            {
                var matches =
                    (from info in container.AvailableServices
                     where info.ServiceType == elementType
                     select info).Count();

                return matches > 0;
            };

            return hasService.Or(hasNamedService);
        }

        /// <summary>
        /// Generates a predicate that determines whether or not a specific type is actually
        /// a list of services that can be created from a given container.
        /// </summary>
        /// <param name="parameterType">The target <see cref="Type"/>. </param>
        /// <returns>A a predicate that determines whether or not a specific type
        /// exists as a list of services in a container</returns>
        public static Func<IServiceContainer, bool> ExistsAsServiceArray(this Type parameterType)
        {
            // The type must be an array
            if (!parameterType.IsArray)
                return container => false;

            var elementType = parameterType.GetElementType();
            // A single service instance implies that a list of services can be created
            // from the current container
            Func<IServiceContainer, bool> hasService = container => container.Contains(elementType);

            // Check for any named services that can be included in the service list
            Func<IServiceContainer, bool> hasNamedService = container =>
                                                                {
                                                                    var matches =
                                                                        (from info in container.AvailableServices
                                                                         where info.ServiceType == elementType
                                                                         select info).Count();

                                                                    return matches > 0;
                                                                };

            return hasService.Or(hasNamedService);
        }

        /// <summary>
        /// Builds an argument list for the <paramref name="method"/>
        /// using the given <paramref name="container"/> instance.
        /// </summary>
        /// <param name="method">The method that will be used to instantiate an object instance.</param>
        /// <param name="container">The container that will provide the method arguments.</param>
        /// <returns>An array of objects to be used with the target method.</returns>
        public static object[] ResolveArgumentsFrom(this MethodBase method,
            IServiceContainer container)
        {
            var resolver = container.GetService<IArgumentResolver>();
            return resolver.ResolveFrom(method, container);
        }


        /// <summary>
        /// Builds an argument list for the target <paramref name="method"/> from
        /// services embedded inside the <paramref name="container"/> instance.
        /// </summary>
        /// <param name="resolver">The <see cref="IArgumentResolver"/> instance that will determine the method arguments.</param>
        /// <param name="method">The target method.</param>
        /// <param name="container">The container that will provide the method arguments.</param>
        /// <param name="additionalArguments">The additional arguments that will be passed to the target method.</param>
        /// <returns>An array of objects to be used with the target method.</returns>
        public static object[] ResolveFrom(this IArgumentResolver resolver, MethodBase method,
            IServiceContainer container, params object[] additionalArguments)
        {
            var parameterTypes = from p in method.GetParameters()
                                 select p.ParameterType;

            return resolver.ResolveFrom(parameterTypes, container, additionalArguments);
        }

    
        /// <summary>
        /// Casts an <see cref="IEnumerable"/> set of items into an array of
        /// <paramref name="targetElementType"/> items.
        /// </summary>
        /// <param name="items">The items being converted.</param>
        /// <param name="targetElementType">The element type of the resulting array.</param>
        /// <returns>An array of items that match the <paramref name="targetElementType"/>.</returns>
        public static object Cast(this IEnumerable items, Type targetElementType)
        {
            var castMethodDefinition = typeof(ResolutionExtensions).GetMethod("Cast", BindingFlags.NonPublic | BindingFlags.Static);
            var castMethod = castMethodDefinition.MakeGenericMethod(targetElementType);

            IEnumerable enumerable = items;
            var arguments = new object[] { enumerable };
            return castMethod.Invoke(null, arguments);
        }

        /// <summary>
        /// Performs a strongly typed cast against an <see cref="IEnumerable"/> instance.
        /// </summary>
        /// <typeparam name="T">The target element type.</typeparam>
        /// <param name="items">The list of items being converted.</param>
        /// <returns>An array of items that match the <typeparamref name="T"/> element type.</returns>
        private static T[] Cast<T>(IEnumerable items)
        {
            var results = new List<T>();
            foreach (var item in items)
            {
                var currentItem = (T)item;
                results.Add(currentItem);
            }

            return results.ToArray();
        }
    }
}
