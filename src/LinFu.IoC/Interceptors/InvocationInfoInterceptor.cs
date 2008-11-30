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
    /// An interceptor that intercepts <see cref="IInvocationInfo"/> instances
    /// and replaces the original target instance with a surrogate instance.
    /// </summary>
    internal class InvocationInfoInterceptor : BaseInterceptor 
    {
        private static readonly MethodInfo _targetMethod;
        private readonly Func<object> _getActualTarget;
        private readonly IInvocationInfo _realInfo;

        static InvocationInfoInterceptor()
        {
            var targetProperty = typeof (IInvocationInfo).GetProperty("Target");
            _targetMethod = targetProperty.GetGetMethod();
        }

        /// <summary>
        /// Initializes the class with a functor that can provide the actual target instance.
        /// </summary>
        /// <param name="getActualTarget">The <see cref="Func{TResult}"/> that will provide the target instance that will be used for the method invocation.</param>
        /// <param name="methodInvoke">The method invoker.</param>
        /// <param name="realInfo">The <see cref="IInvocationInfo"/> instance that describes the current execution context.</param>
        internal InvocationInfoInterceptor(IInvocationInfo realInfo, Func<object> getActualTarget, 
            IMethodInvoke<MethodInfo> methodInvoke) : base(methodInvoke)
        {
            _getActualTarget = getActualTarget;
            _realInfo = realInfo;
        }

        public override object Intercept(IInvocationInfo info)
        {
            var targetMethod = info.TargetMethod;

            // Intercept calls made only to the IInvocationInfo interface
            if (targetMethod.DeclaringType  != typeof(IInvocationInfo) || targetMethod.Name != "get_Target")
                return base.Intercept(info);

            var target = _getActualTarget();

            // Replace the proxy with the actual target           
            return target;
        }

        /// <summary>
        /// Gets the target object instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that describes the current execution context.</param>
        protected override object GetTarget(IInvocationInfo info)
        {
            return _realInfo;
        }
    }
}
