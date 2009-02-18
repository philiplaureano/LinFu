using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.IoC.Reflection;
using LinFu.Proxy.Interfaces;

namespace LinFu.Proxy
{
    /// <summary>
    /// A class that adds proxy support to the <see cref="object"/> class
    /// </summary>
    public static class ObjectExtensions
    {
        private static readonly IServiceContainer _container = new ServiceContainer();

        static ObjectExtensions()
        {
            _container.LoadFromBaseDirectory("*.dll");
        }

        /// <summary>
        /// Creates a duck type that redirects its calls to the
        /// given <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The target instance that will be invoked once the duck type instance has been invoked.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that will be implemented by the duck type.</param>
        /// <typeparam name="T">The type parameter that describes the duck type.</typeparam>
        /// <returns>The return value from the target method.</returns>
        public static T CreateDuck<T>(this object target, params Type[] baseInterfaces)
        {
            return (T)target.CreateDuck(typeof(T), baseInterfaces);
        }

        /// <summary>
        /// Creates a duck type that redirects its calls to the
        /// given <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The target instance that will be invoked once the duck type instance has been invoked.</param>
        /// <param name="duckType">The <see cref="System.Type"/> that describes the duck type.</param>
        /// <param name="baseInterfaces">The additional list of interfaces that will be implemented by the duck type.</param>
        /// <returns>The return value from the target method.</returns>
        public static object CreateDuck(this object target, Type duckType, params Type[] baseInterfaces)
        {
            Func<string, Type[], object[], object> implementation
                = (methodName, typeArguments, arguments) =>
                      target.Invoke(methodName, typeArguments, arguments);

            { };
            var proxyFactory = _container.GetService<IProxyFactory>();
            return proxyFactory.CreateProxy(duckType, implementation, baseInterfaces);
        }
    }
}
