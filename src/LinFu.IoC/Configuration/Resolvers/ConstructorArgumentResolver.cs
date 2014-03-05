using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Resolvers
{
    /// <summary>
    /// Represents a class that determines the method arguments that should be used for a given constructor.
    /// </summary>
    public class ConstructorArgumentResolver : IConstructorArgumentResolver, IInitialize
    {
        private IArgumentResolver _argumentResolver;


        /// <summary>
        /// Determines the parameter values that should be used for a given constructor.
        /// </summary>
        /// <param name="constructor">The target constructor.</param>
        /// <param name="container">The host container instance.</param>
        /// <param name="additionalArguments">The list of additional arguments that should be combined with the arguments from the container.</param>
        /// <returns>A list of arguments that will be used for the given constructor.</returns>
        public object[] GetConstructorArguments(ConstructorInfo constructor, IServiceContainer container,
            object[] additionalArguments)
        {
            var parameterTypes = GetMissingParameterTypes(constructor, additionalArguments);

            // Generate the arguments for the target constructor
            return _argumentResolver.ResolveFrom(parameterTypes, container,
                additionalArguments);
        }


        /// <summary>
        /// Initializes the class with the default services.
        /// </summary>
        /// <param name="container">The target service container.</param>
        public void Initialize(IServiceContainer container)
        {
            _argumentResolver = container.GetService<IArgumentResolver>();
        }


        /// <summary>
        /// Determines which parameter types need to be supplied to invoke a particular
        /// <paramref name="constructor"/>  instance.
        /// </summary>
        /// <param name="constructor">The target constructor.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to invoke the constructor.</param>
        /// <returns>The list of parameter types that are still missing parameter values.</returns>
        private static IEnumerable<INamedType> GetMissingParameterTypes(ConstructorInfo constructor,
            IEnumerable<object> additionalArguments)
        {
            var parameters = from p in constructor.GetParameters()
                select p;

            // Determine which parameters need to 
            // be supplied by the container
            var parameterTypes = new List<INamedType>();
            var argumentCount = additionalArguments.Count();
            if (additionalArguments != null && argumentCount > 0)
            {
                // Supply parameter values for the
                // parameters that weren't supplied by the
                // additionalArguments
                var parameterCount = parameters.Count();
                var maxIndex = parameterCount - argumentCount;
                var targetParameters = from param in parameters.Where(p => p.Position < maxIndex)
                    select new NamedType(param) as INamedType;

                parameterTypes.AddRange(targetParameters);
                return parameterTypes;
            }

            var results = from param in parameters
                select new NamedType(param) as INamedType;

            parameterTypes.AddRange(results);

            return parameterTypes;
        }
    }
}