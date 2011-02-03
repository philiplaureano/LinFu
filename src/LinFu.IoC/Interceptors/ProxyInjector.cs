using System;
using LinFu.IoC.Interfaces;
using LinFu.Proxy.Interfaces;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// Represents a class that automatically injects a proxy instance
    /// instead of an actual service instance.
    /// </summary>
    public class ProxyInjector : IPostProcessor
    {
        private readonly Func<IServiceRequestResult, object> _createProxy;
        private readonly Func<IServiceRequestResult, bool> _filterPredicate;

        /// <summary>
        /// Initializes the class with the <paramref name="filterPredicate"/>
        /// and the <paramref name="createProxy"/> factory method.
        /// </summary>
        /// <param name="filterPredicate">The predicate that will determine which service requests will be proxied.</param>
        /// <param name="createProxy">The factory method that will generate the proxy instance itself.</param>
        public ProxyInjector(Func<IServiceRequestResult, bool> filterPredicate,
                               Func<IServiceRequestResult, object> createProxy)
        {
            _filterPredicate = filterPredicate;
            _createProxy = createProxy;
        }

        #region IPostProcessor Members

        /// <summary>
        /// A method that injects service proxies in place of the actual <see cref="IServiceRequestResult.ActualResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> instance that describes the service request.</param>
        public void PostProcess(IServiceRequestResult result)
        {
            if (!_filterPredicate(result))
                return;

            IServiceContainer container = result.Container;
            bool hasProxyFactory = container.Contains(typeof (IProxyFactory));

            // Inject proxies only if a
            // proxy factory instance is available
            if (!hasProxyFactory)
                return;

            // Sealed types cannot be intercepted
            Type serviceType = result.ServiceType;
            if (result.ActualResult != null && serviceType.IsSealed)
                return;

            // Replace the actual result with the proxy itself
            result.ActualResult = _createProxy(result);
        }

        #endregion
    }
}