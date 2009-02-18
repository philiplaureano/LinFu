using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// A class that converts a functor into an <see cref="IInterceptor"/> instance.
    /// </summary>
    public class FunctorAsInterceptor : IInterceptor
    {
        private readonly Func<IInvocationInfo, object> _doIntercept;
        
        /// <summary>
        /// Initializes the class with the given <paramref name="intercept">functor</paramref>.
        /// </summary>
        /// <param name="intercept">The functor that will be invoked every time a method is called on the proxy type.</param>
        public FunctorAsInterceptor(Func<IInvocationInfo, object> intercept)
        {
            _doIntercept = intercept;
        }

        /// <summary>
        /// A method that redirects the method calls to 
        /// the functor instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that describes the context of the method call.</param>
        /// <returns>The return value from the target method.</returns>
        public object Intercept(IInvocationInfo info)
        {
            if (_doIntercept == null)
                throw new NotImplementedException();

            return _doIntercept(info);
        }
    }
}