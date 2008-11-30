using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a type that can invoke a method
    /// using a given set of method arguments.
    /// </summary>
    public interface IMethodInvoke<TMethod>
        where TMethod : MethodBase
    {
        /// <summary>
        /// Invokes the <paramref name="targetMethod"/>
        /// using the given <paramref name="arguments"/>.
        /// </summary>
        /// <param name="target">The target object instance.</param>
        /// <param name="targetMethod">The target method to invoke.</param>
        /// <param name="arguments">The arguments to be used with the method.</param>
        /// <returns>The method return value.</returns>
        object Invoke(object target, TMethod targetMethod, params object[] arguments);
    }
}