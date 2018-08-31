using System;
using System.Reflection;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    ///     Represents a class that can instantiate object instances.
    /// </summary>
    public class DefaultActivator : IActivator<IContainerActivationContext>, IInitialize
    {
        private IConstructorArgumentResolver _argumentResolver;
        private IMethodInvoke<ConstructorInfo> _constructorInvoke;
        private IMemberResolver<ConstructorInfo> _resolver;


        /// <summary>
        ///     Creates an object instance.
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
            var arguments = _argumentResolver.GetConstructorArguments(constructor, container, additionalArguments);

            // Instantiate the object
            var result = _constructorInvoke.Invoke(null, constructor, arguments);

            return result;
        }


        /// <summary>
        ///     Initializes the class with the default services.
        /// </summary>
        /// <param name="container">The target service container.</param>
        public void Initialize(IServiceContainer container)
        {
            _resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            _constructorInvoke = container.GetService<IMethodInvoke<ConstructorInfo>>();
            _argumentResolver = container.GetService<IConstructorArgumentResolver>();
        }
    }
}