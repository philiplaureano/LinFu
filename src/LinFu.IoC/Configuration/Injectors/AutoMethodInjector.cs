using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
using System.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that automatically invokes methods using arguments
    /// derived from existing instances from within a <see cref="IServiceContainer"/>
    /// instance.
    /// </summary>
    public class AutoMethodInjector : AutoMemberInjector<MethodInfo>
    {
        /// <summary>
        /// Injects services from the <paramref name="container"/> into the target <see cref="MethodInfo"/> instance.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="method">The <see cref="MethodInfo"/> instance that will store the service instance.</param>
        /// <param name="resolver">The <see cref="IArgumentResolver"/> that will determine which arguments will be assigned to the target member.</param>
        /// <param name="additionalArguments">The additional arguments that were passed to the <see cref="IServiceRequestResult"/> during the instantiation process.</param>
        /// <param name="container">The container that will provide the service instances.</param>
        protected override void Inject(object target, MethodInfo method, 
            IArgumentResolver resolver, IServiceContainer container, object[] additionalArguments)
        {
            var parameterTypes = from p in method.GetParameters()
                                 select p.ParameterType;

            var arguments = resolver.ResolveFrom(parameterTypes, container, additionalArguments);

            // Invoke the target method
            method.Invoke(target, arguments);
        }
    }
}
