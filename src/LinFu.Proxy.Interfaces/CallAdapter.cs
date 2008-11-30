using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.Proxy
{
    /// <summary>
    /// Adapts an <see cref="IInvokeWrapper"/> instance to an
    /// <see cref="IInterceptor"/> instance.
    /// </summary>
    internal class CallAdapter : IInterceptor
    {
        private readonly IInvokeWrapper _wrapper;

        /// <summary>
        /// Initializes the CallAdapter class with the <paramref name="wrapper"/> instance.
        /// </summary>
        /// <param name="wrapper">The <see cref="IInvokeWrapper"/> instance that will be called every time the interceptor is invoked.</param>
        public CallAdapter(IInvokeWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        /// <summary>
        /// Intercepts a method call and passes the <see cref="IInvocationInfo"/> arguments
        /// down to the <see cref="IInvokeWrapper"/> instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that describes the method currently being executed.</param>
        /// <returns>The return value of the target method.</returns>
        public object Intercept(IInvocationInfo info)
        {
            object result = null;

            // Signal the beginning of the method call
            _wrapper.BeforeInvoke(info);

            // Process the method call itself
            result = _wrapper.DoInvoke(info);

            // Postprocess the results
            _wrapper.AfterInvoke(info, result);

            return result;
        }
    }
}
