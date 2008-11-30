using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// A <see cref="IContainerPlugin"/> implementation that inserts
    /// <see cref="ProxyInjector"/> instances at the beginning of a <see cref="IServiceContainer"/>
    /// loading sequence.
    /// </summary>
    internal class ProxyContainerPlugin : IContainerPlugin
    {
        private readonly ProxyInjector _injector;

        /// <summary>
        /// Initializes the class with the given <see cref="ProxyInjector"/> instance.
        /// </summary>
        /// <param name="injector">The postprocessor that will inject proxies in place of actual service requests.</param>
        internal ProxyContainerPlugin(ProxyInjector injector)
        {
            _injector = injector;
        }

        /// <summary>
        /// Injects a <see cref="ProxyInjector"/> into the <paramref name="target">target container</paramref>.
        /// </summary>
        /// <param name="target">The service container that will hold the <see cref="ProxyInjector"/>.</param>        
        public void BeginLoad(IServiceContainer target)
        {
            target.PostProcessors.Add(_injector);
        }

        /// <summary>
        /// Does absolutely nothing.
        /// </summary>
        /// <param name="target">The target container.</param>
        public void EndLoad(IServiceContainer target)
        {
            // Do nothing
        }
    }
}
