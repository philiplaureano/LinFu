using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.AOP
{
    /// <summary>
    /// Extends <see cref="IInvocationInfo"/> interface
    /// to make it easier to use.
    /// </summary>
    public static class InvocationInfoExtensions
    {
        /// <summary>
        /// Invokes the currently executing method by using the <see cref="IInvocationInfo.Target"/>
        /// as the target instance, the <see cref="IInvocationInfo.TargetMethod"/> as the method, 
        /// and uses the <see cref="IInvocationInfo.Arguments"/> for the method
        /// arguments.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that contains information about the method call itself.</param>
        /// <returns>The return value of the method call.</returns>
        public static object Proceed(this IInvocationInfo info)
        {
            var targetMethod = info.TargetMethod;
            var target = info.Target;
            var arguments = info.Arguments;

            return targetMethod.Invoke(target, arguments);
        }

        /// <summary>
        /// Invokes the currently executing method by using the <paramref name="target"/>
        /// as the target instance, the <see cref="IInvocationInfo.TargetMethod"/> as the method, 
        /// and uses the <see cref="IInvocationInfo.Arguments"/> for the method
        /// arguments.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that contains information about the method call itself.</param>
        /// <param name="target">The target instance that will handle the method call.</param>
        /// <returns>The return value of the method call.</returns>
        public static object Proceed(this IInvocationInfo info, object target)
        {
            var targetMethod = info.TargetMethod;
            var arguments = info.Arguments;

            return targetMethod.Invoke(target, arguments);
        }

        /// <summary>
        /// Invokes the currently executing method by using the <paramref name="target"/>
        /// as the target instance, the <see cref="IInvocationInfo.TargetMethod"/> as the method, 
        /// and uses the <paramref name="arguments"/> for the method
        /// arguments.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that contains information about the method call itself.</param>
        /// <param name="target">The target instance that will handle the method call.</param>
        /// <param name="arguments">The arguments that will be used for the actual method call.</param>
        /// <returns>The return value of the method call.</returns>
        public static object Proceed(this IInvocationInfo info, object target, 
            params object[] arguments)
        {
            var targetMethod = info.TargetMethod;        
            return targetMethod.Invoke(target, arguments);
        }
    }
}
