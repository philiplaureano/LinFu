using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.Proxy.Interfaces;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// An interceptor class that redirects calls to another interceptor.
    /// </summary>
    internal class Redirector : BaseInterceptor
    {
        private readonly Func<object> _getActualTarget;
        private readonly IInterceptor _interceptor;
        private readonly IProxyFactory _proxyFactory;
        public Redirector(Func<object> getActualTarget, IInterceptor targetInterceptor, IProxyFactory factory,
            IMethodInvoke<MethodInfo> methodInvoke)
            : base(methodInvoke)
        {
            _getActualTarget = getActualTarget;
            _interceptor = targetInterceptor;
            _proxyFactory = factory;
        }

        public override object Intercept(IInvocationInfo info)
        {
            // Instead of using the proxy as the target,
            // modify the InvocationInfo to show the actual target
            var proxyType = _proxyFactory.CreateProxyType(typeof(IInvocationInfo), new Type[0]);
            var infoProxy = Activator.CreateInstance(proxyType) as IProxy;

            if (infoProxy == null)
                return base.Intercept(info);

            var modifiedInfo = (IInvocationInfo)infoProxy;

            // Replace the proxy target with the actual target
            var infoInterceptor = new InvocationInfoInterceptor(info, _getActualTarget, MethodInvoker);
            infoProxy.Interceptor = infoInterceptor;

            return _interceptor.Intercept(modifiedInfo);
        }

        /// <summary>
        /// Gets the target object instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that describes the current execution context.</param>
        protected override object GetTarget(IInvocationInfo info)
        {
            return _getActualTarget();
        }
    }
}
