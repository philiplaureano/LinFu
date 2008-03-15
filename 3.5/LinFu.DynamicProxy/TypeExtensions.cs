using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;
using System.Reflection;
namespace LinFu.DynamicProxy
{
    public static class TypeExtensions
    {
        private static readonly ProxyFactory _factory = new ProxyFactory();
        public static IProxy CreateProxy(this Type type, params Type[] baseInterfaces)
        {
            Type proxyType = _factory.CreateProxyType(type, baseInterfaces);
            IProxy result = (IProxy)Activator.CreateInstance(proxyType);

            return result;
        }
        public static IProxy CreateProxy(this Type type, 
            IInterceptor interceptor, params Type[] baseInterfaces)
        {
            IProxy proxy = type.CreateProxy(baseInterfaces);
            proxy.Interceptor = interceptor;

            return proxy;
        }
        public static IProxy CreateProxy(this Type type, IInvokeWrapper wrapper, params Type[] baseInterfaces)
        {
            Type proxyType = _factory.CreateProxyType(type, baseInterfaces);
            IProxy result = (IProxy)Activator.CreateInstance(proxyType);
            result.Interceptor = new CallAdapter(wrapper);
            return result;
        }
        public static bool CanBeProxied(this Type type)
        {
            if (type.IsSealed)
                return false;
            
            if (!type.IsClass)
                return false;

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            // Search for any methods that cannot be proxied
            var matches = from m in type.GetMethods(flags)
                          where !(m.IsAbstract && m.IsVirtual) || m.IsFinal
                          select m;

            // A type can only be proxied if all of its public methods are virtual
            return matches.Count() == 0;
        }
    }
}
