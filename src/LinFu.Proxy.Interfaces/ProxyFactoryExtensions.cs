using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Proxy.Interfaces;

namespace LinFu.Proxy.Interfaces
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
            return (T)factory.CreateProxy(typeof(T), wrapper, baseInterfaces);
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
            return (T)factory.CreateProxy(typeof(T), interceptor, baseInterfaces);
        }

        /// <summary>
        /// Uses the <paramref name="proxyFactory"/> to create a proxy instance
        /// that directly derives from the <typeparamref name="T"/> type
        /// and implements the given <paramref name="baseInterfaces"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="proxyImplementation"/> will be used to intercept method calls
        /// performed against the target instance.
        /// </remarks>
        /// <typeparam name="T">The type that will be intercepted by the proxy.</typeparam>
        /// <param name="proxyFactory">The IProxyFactory instance that will be used to generate the proxy type.</param>
        /// <param name="proxyImplementation">The functor that will invoked every time a method is called on the proxy.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that the proxy will implement.</param>
        /// <returns>A valid proxy instance.</returns>
        public static T CreateProxy<T>(this IProxyFactory proxyFactory, 
            Func<string, Type[], object[], object> proxyImplementation, params Type[] baseInterfaces)
        {
            var targetType = typeof(T);
            object result = CreateProxy(proxyFactory, targetType, proxyImplementation, baseInterfaces);

            return (T)result;
        }

        /// <summary>
        /// Uses the <paramref name="proxyFactory"/> to create a proxy instance
        /// that directly derives from the <typeparamref name="T"/> type
        /// and implements the given <paramref name="baseInterfaces"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="proxyImplementation"/> will be used to intercept method calls
        /// performed against the target instance.
        /// </remarks>
        /// <param name="targetType">The type that will be intercepted by the proxy.</param>
        /// <param name="proxyFactory">The IProxyFactory instance that will be used to generate the proxy type.</param>
        /// <param name="proxyImplementation">The functor that will invoked every time a method is called on the proxy.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that the proxy will implement.</param>
        /// <returns>A valid proxy instance.</returns>
        public static object CreateProxy(this IProxyFactory proxyFactory, Type targetType,
            Func<string, Type[], object[], object> proxyImplementation, params Type[] baseInterfaces)
        {
            Func<IInvocationInfo, object> doIntercept = info =>
            {
                var targetMethod = info.TargetMethod;
                var methodName = targetMethod.Name;
                var arguments = info.Arguments;
                Type[] typeArguments = info.TypeArguments;

                return proxyImplementation(methodName, typeArguments, arguments);
            };

            var interceptor = new FunctorAsInterceptor(doIntercept);
            return proxyFactory.CreateProxy(targetType, interceptor, baseInterfaces);
        }
    }
}
