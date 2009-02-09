using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that provides <see cref="IInterceptor"/> instances
    /// that execute in place of an actual target method.
    /// </summary>
    public interface IInterceptorProvider
    {
        /// <summary>
        /// Determines which interceptor should be used for the given
        /// <paramref name="context"/> parameter.
        /// </summary>
        /// <param name="context">The <see cref="IInvocationInfo"/> that describes the context of the method call at the call site.</param>
        /// <returns>An <see cref="IInterceptor"/> instance if the method should be intercepted; otherwise, it should return <c>false</c>.</returns>
        IInterceptor GetInterceptor(IInvocationInfo context);
    }
}
