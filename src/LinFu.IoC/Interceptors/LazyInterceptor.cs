using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// An interceptor class that instantiates a target type only when
    /// the methods for that target are invoked.
    /// </summary>
    /// <typeparam name="T">The type of object to intercept.</typeparam>
    public class LazyInterceptor<T> : BaseInterceptor
        where T: class
    {
        private readonly Func<T> _getInstance;

        /// <summary>
        /// Initializes the class with the <paramref name="getInstance"/>
        /// factory method.
        /// </summary>
        /// <param name="getInstance">The functor that will be used to create the actual object instance.</param>
        public LazyInterceptor(Func<T> getInstance)
        {
            _getInstance = getInstance;    
        }

        /// <summary>
        /// A method that uses the given factory method to provide a target
        /// for the method currently being invoked.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> object that describes the current invocation context.</param>
        /// <returns>The target itself.</returns>
        protected override object GetTarget(IInvocationInfo info)
        {
            return _getInstance();
        }

        /// <summary>
        /// Intercepts the method and initializes the target instance before the 
        /// actual object is invoked.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> that describes the execution context.</param>
        /// <returns>The return value of the target method.</returns>
        public override object Intercept(IInvocationInfo info)
        {
            var target = _getInstance();
            var arguments = info.Arguments;
            var method = info.TargetMethod;

            object result = null;
            try
            {
                result = method.Invoke(target, arguments);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            
            return result;
        }
    }
}
