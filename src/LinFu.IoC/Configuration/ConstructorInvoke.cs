using System;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    ///     A class that invokes constructor instances.
    /// </summary>
    public class ConstructorInvoke : IMethodInvoke<ConstructorInfo>
    {
        /// <summary>
        ///     Invokes the <paramref name="targetMethod" /> constructor
        ///     using the given <paramref name="arguments" />.
        /// </summary>
        /// <param name="target">The target object instance.</param>
        /// <param name="targetMethod">The target method to invoke.</param>
        /// <param name="arguments">The arguments to be used with the method.</param>
        /// <returns>The method return value.</returns>
        public object Invoke(object target, ConstructorInfo targetMethod, params object[] arguments)
        {
            var declaringType = targetMethod.DeclaringType;
            return Activator.CreateInstance(declaringType, arguments);
        }
    }
}