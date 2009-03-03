using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that can instantiate object instances.
    /// </summary>
    public class DefaultActivator : IActivator<IContainerActivationContext>, IInitialize 
    {
        private IArgumentResolver _argumentResolver;
        private IMemberResolver<ConstructorInfo> _resolver;
        private IMethodInvoke<ConstructorInfo> _constructorInvoke;

        /// <summary>
        /// Creates an object instance.
        /// </summary>
        /// <returns>A valid object instance.</returns>
        public object CreateInstance(IContainerActivationContext context)
        {
            var container = context.Container;
            var additionalArguments = context.AdditionalArguments;
            var concreteType = context.TargetType;

            // Add the required services if necessary
            container.AddDefaultServices();

            var finderContext = new MethodFinderContext(new Type[0], additionalArguments, null);

            // Determine which constructor
            // contains the most resolvable
            // parameters            
            var constructor = _resolver.ResolveFrom(concreteType, container, finderContext);

            // TODO: Allow users to insert their own custom constructor resolution routines here
            var parameterTypes = GetMissingParameterTypes(constructor, finderContext.Arguments);

            // Generate the arguments for the target constructor
            var arguments = _argumentResolver.ResolveFrom(parameterTypes, container,
                                                         additionalArguments);
            // Instantiate the object
            var result = _constructorInvoke.Invoke(null, constructor, arguments);

            return result;
        }

        /// <summary>
        /// Determines which parameter types need to be supplied to invoke a particular
        /// <paramref name="constructor"/>  instance.
        /// </summary>
        /// <param name="constructor">The target constructor.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to invoke the constructor.</param>
        /// <returns>The list of parameter types that are still missing parameter values.</returns>
        private static List<Type> GetMissingParameterTypes(ConstructorInfo constructor,
            IEnumerable<object> additionalArguments)
        {
            var parameters = from p in constructor.GetParameters()
                             select new { p.Position, Type = p.ParameterType };

            // Determine which parameters need to 
            // be supplied by the container
            var parameterTypes = new List<Type>();
            var argumentCount = additionalArguments.Count();
            if (additionalArguments != null && argumentCount > 0)
            {
                // Supply parameter values for the
                // parameters that weren't supplied by the
                // additionalArguments
                var parameterCount = parameters.Count();
                var maxIndex = parameterCount - argumentCount;
                var targetParameters = from param in parameters.Where(p => p.Position < maxIndex)
                                       select param.Type;

                parameterTypes.AddRange(targetParameters);
                return parameterTypes;
            }

            var results = from param in parameters
                          select param.Type;

            parameterTypes.AddRange(results);

            return parameterTypes;
        }

        /// <summary>
        /// Initializes the class with the default services.
        /// </summary>
        /// <param name="container">The target service container.</param>
        public void Initialize(IServiceContainer container)
        {
            _argumentResolver = container.GetService<IArgumentResolver>();
            _resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            _constructorInvoke = container.GetService<IMethodInvoke<ConstructorInfo>>();
        }
    }
}
