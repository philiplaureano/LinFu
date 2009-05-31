using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that can swap method body implementations at runtime.
    /// </summary>
    public interface IMethodReplacementProvider
    {
        /// <summary>
        /// Determines whether or not the current method implementation can be replaced.
        /// </summary>
        /// <param name="host">The target instance of the method call.</param>
        /// <param name="info">The <see cref="IInvocationInfo"/> that describes the context of the method call.</param>
        /// <returns><c>true</c> if the method can be intercepted; otherwise, it will return <c>false</c>.</returns>
        bool CanReplace(object host, IInvocationInfo info);

        /// <summary>
        /// Obtains the <see cref="IInterceptor"/> instance that will be used to replace the current method call.
        /// </summary>
        /// <param name="host">The target instance of the method call.</param>
        /// <param name="info">The <see cref="IInvocationInfo"/> that describes the context of the method call.</param>
        /// <returns>The interceptor that will intercept the method call itself.</returns>
        IInterceptor GetMethodReplacement(object host, IInvocationInfo info);
    }
}
