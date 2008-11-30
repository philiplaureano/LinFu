using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Proxy.Interfaces;

namespace LinFu.Proxy
{
    /// <summary>
    /// Extends the <see cref="IProxyFactory"/> class to support
    /// instantiating proxy types.
    /// </summary>
    public static class ProxyFactoryExtensions
    {
        /// <summary>
        /// Uses the <paramref name="factory"/> to create a proxy instance 
        /// that directly derives from the <paramref name="instanceType"/> 
        /// and implements the given <paramref name="baseInterfaces"/>.
        /// The <paramref name="wrapper"/> instance, in turn, will be used
        /// to intercept the method calls made to the proxy itself.
        /// </summary>
        /// <param name="factory">The IProxyFactory instance that will be used to generate the proxy type.</param>
        /// <param name="instanceType">The type that will be intercepted by the proxy.</param>
        /// <param name="wrapper">The <see cref="IInvokeWrapper"/> instance that will be used to intercept method calls made to the proxy.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that the proxy will implement.</param>
        /// <returns>A valid proxy instance.</returns>
        public static object CreateProxy(this IProxyFactory factory, Type instanceType, 
            IInvokeWrapper wrapper, params Type[] baseInterfaces)
        {
            // Convert the wrapper to an IInterceptor instance.
            var adapter = new CallAdapter(wrapper);
            return factory.CreateProxy(instanceType, adapter, baseInterfaces);
        }

        /// <summary>
        /// Uses the <paramref name="factory"/> to create a proxy instance 
        /// that directly derives from the <paramref name="instanceType"/> 
        /// and implements the given <paramref name="baseInterfaces"/>.
        /// The <paramref name="interceptor"/> instance, in turn, will be used
        /// to intercept the method calls made to the proxy itself.
        /// </summary>
        /// <param name="factory">The IProxyFactory instance that will be used to generate the proxy type.</param>
        /// <param name="instanceType">The type that will be intercepted by the proxy.</param>
        /// <param name="interceptor">The <see cref="IInterceptor"/> instance that will be used to intercept method calls made to the proxy.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that the proxy will implement.</param>
        /// <returns>A valid proxy instance.</returns>
        public static object CreateProxy(this IProxyFactory factory, Type instanceType, 
            IInterceptor interceptor, params Type[] baseInterfaces)
        {
            var proxyType = factory.CreateProxyType(instanceType, baseInterfaces);
            var proxyInstance = (IProxy)Activator.CreateInstance(proxyType);

            proxyInstance.Interceptor = interceptor;

            return proxyInstance;
        }

        /// <summary>
        /// Uses the <paramref name="factory"/> to create a proxy instance 
        /// that directly derives from the <typeparamref name="T"/> type
        /// and implements the given <paramref name="baseInterfaces"/>.
        /// The <paramref name="wrapper"/> instance, in turn, will be used
        /// to intercept the method calls made to the proxy itself.
        /// </summary>
        /// <typeparam name="T">The type that will be intercepted by the proxy.</typeparam>
        /// <param name="factory">The IProxyFactory instance that will be used to generate the proxy type.</param>        
        /// <param name="wrapper">The <see cref="IInvokeWrapper"/> instance that will be used to intercept method calls made to the proxy.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that the proxy will implement.</param>
        /// <returns>A valid proxy instance.</returns>
        public static T CreateProxy<T>(this IProxyFactory factory, IInvokeWrapper wrapper, 
            params Type[] baseInterfaces)
        {
            return (T) factory.CreateProxy(typeof (T), wrapper, baseInterfaces);
        }

        /// <summary>
        /// Uses the <paramref name="factory"/> to create a proxy instance 
        /// that directly derives from the <typeparamref name="T"/> type
        /// and implements the given <paramref name="baseInterfaces"/>.
        /// The <paramref name="interceptor"/> instance, in turn, will be used
        /// to intercept the method calls made to the proxy itself.
        /// </summary>
        /// <typeparam name="T">The type that will be intercepted by the proxy.</typeparam>
        /// <param name="factory">The IProxyFactory instance that will be used to generate the proxy type.</param>        
        /// <param name="interceptor">The <see cref="IInterceptor"/> instance that will be used to intercept method calls made to the proxy.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that the proxy will implement.</param>
        /// <returns>A valid proxy instance.</returns>
        public static T CreateProxy<T>(this IProxyFactory factory, IInterceptor interceptor, 
            params Type[] baseInterfaces)
        {
            return (T) factory.CreateProxy(typeof (T), interceptor, baseInterfaces);
        }
    }
}
