using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// Represents a dynamically generated proxy instance.
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// The interceptor that will handle all
        /// calls made to the proxy instance.
        /// </summary>
        IInterceptor Interceptor { get; set; }
    }
}
