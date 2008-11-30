using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// Adapts a <see cref="IAroundInvoke"/> instance into an <see cref="IInterceptor"/>.
    /// </summary>
    internal class AroundInvokeAdapter : BaseInterceptor
    {
        private readonly IAroundInvoke _wrapper;
        private readonly Func<object> _getTarget;

        /// <summary>
        /// Initializes the <see cref="AroundInvokeAdapter"/> class.
        /// </summary>
        /// <param name="getTarget">The functor responsible for obtaining the target instance.</param>
        /// <param name="methodInvoke">The method invoker.</param>
        /// <param name="aroundInvoke">The target <see cref="IAroundInvoke"/> instance.</param>
        internal AroundInvokeAdapter(Func<object> getTarget, IMethodInvoke<MethodInfo> methodInvoke,
            IAroundInvoke aroundInvoke)
            : base(methodInvoke)
        {
            _wrapper = aroundInvoke;
            _getTarget = getTarget;
        }

        /// <summary>
        /// Converts the call to <see cref="IInterceptor.Intercept"/> to an
        /// <see cref="IAroundInvoke"/> method call.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> that describes the context of the method call.</param>
        /// <returns>The return value from the target method.</returns>
        public override object Intercept(IInvocationInfo info)
        {
            object result = null;

            // Signal the beginning of the method call
            _wrapper.BeforeInvoke(info);

            // Call the target method
            result = base.Intercept(info);

            // Postprocess the results
            _wrapper.AfterInvoke(info, result);

            return result;
        }

        /// <summary>
        /// Gets the target object instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that describes the current execution context.</param>
        protected override object GetTarget(IInvocationInfo info)
        {
            return _getTarget();
        }
    }
}
