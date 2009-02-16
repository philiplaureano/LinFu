using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Reflection
{
    /// <summary>
    /// A class that adds late binding support to any CLR object.
    /// </summary>
    public static class LateBoundExtensions
    {
        private static readonly IServiceContainer _container = new ServiceContainer();

        static LateBoundExtensions()
        {
            _container.LoadFromBaseDirectory("*.dll");
        }

        /// <summary>
        /// Invokes a method on the target <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The target instance that will be used to invoke the method.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <param name="arguments">The arguments that will be passed to the target method.</param>
        /// <returns>The method return value.</returns>
        public static object Invoke(this object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
                throw new NullReferenceException("instance");

            var context = new MethodFinderContext(arguments);
            return Invoke(instance, methodName, context);
        }

        /// <summary>
        /// Invokes a method on the target <paramref name="instance"/> using the given <paramref name="methodName"/> and <paramref name="context"/>.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <param name="context">The <see cref="IMethodFinderContext"/> that describes the target method.</param>
        /// <returns>The method return value.</returns>
        public static object Invoke(this object instance, string methodName, MethodFinderContext context)
        {
            var targetType = instance.GetType();
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            // Group the methods by name
            var methodMap = new Dictionary<string, List<MethodInfo>>();
            var methods = targetType.GetMethods(flags);
            foreach (var method in methods)
            {
                var name = method.Name;
                if (!methodMap.ContainsKey(name))
                    methodMap[name] = new List<MethodInfo>();

                var currentList = methodMap[name];
                currentList.Add(method);
            }

            var targetMethods = methodMap.ContainsKey(methodName) ? methodMap[methodName] : (new MethodInfo[0]).ToList();
            var finder = _container.GetService<IMethodFinder<MethodInfo>>();

            var targetMethod = finder.GetBestMatch(targetMethods, context);

            // Search the methods that match the given method name
            if (targetMethod == null || targetMethods.Count == 0)
            {
                var message = string.Format("Method '{0}' not found on type '{1}'", methodName, targetType);
                throw new ArgumentException(message, "methodName");
            }

            // Instantiate the generic method, if necessary
            if (targetMethod.ContainsGenericParameters)
                targetMethod = targetMethod.MakeGenericMethod(context.TypeArguments.ToArray());

            var invoker = _container.GetService<IMethodInvoke<MethodInfo>>();

            return invoker.Invoke(instance, targetMethod, context.Arguments.ToArray());
        }

        /// <summary>
        /// Invokes a method on the target <paramref name="instance"/> using the given <paramref name="methodName"/>.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <typeparam name="T1">The type argument that will be passed to the target method</typeparam>.
        /// <param name="arguments">The arguments that will be passed to the target method.</param>
        /// <returns>The method return value.</returns>
        public static object Invoke<T1>(this object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
                throw new NullReferenceException("instance");

            var context = new MethodFinderContext(new Type[] { typeof(T1) }, arguments, null);
            return Invoke(instance, methodName, context);
        }

        /// <summary>
        /// Invokes a method on the target <paramref name="instance"/> using the given <paramref name="methodName"/>.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <typeparam name="T1">The first type argument that will be passed to the target method</typeparam>.
        /// <typeparam name="T2">The second type argument that will be passed to the target method</typeparam>.
        /// <param name="arguments">The arguments that will be passed to the target method.</param>
        /// <returns>The method return value.</returns>
        public static object Invoke<T1, T2>(this object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
                throw new NullReferenceException("instance");

            var typeArguments = new Type[] {typeof (T1), typeof (T2)};
            return Invoke(instance, methodName, typeArguments, arguments);
        }

        /// <summary>
        /// Invokes a method on the target <paramref name="instance"/> using the given <paramref name="methodName"/>.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <typeparam name="T1">The first type argument that will be passed to the target method</typeparam>.
        /// <typeparam name="T2">The second type argument that will be passed to the target method</typeparam>.
        /// <typeparam name="T3">The third type argument that will be passed to the target method.</typeparam>
        /// <param name="arguments">The arguments that will be passed to the target method.</param>
        /// <returns>The method return value.</returns>
        public static object Invoke<T1, T2, T3>(this object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
                throw new NullReferenceException("instance");

            var typeArguments = new Type[] { typeof(T1), typeof(T2), typeof(T3) };
            return Invoke(instance, methodName, typeArguments, arguments);
        }

        /// <summary>
        /// Invokes a method on the target <paramref name="instance"/> using the given <paramref name="methodName"/>.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <typeparam name="T1">The first type argument that will be passed to the target method</typeparam>.
        /// <typeparam name="T2">The second type argument that will be passed to the target method</typeparam>.
        /// <typeparam name="T3">The third type argument that will be passed to the target method.</typeparam>
        /// <typeparam name="T4">The fourth type argument that will be passed to the target method.</typeparam>
        /// <param name="arguments">The arguments that will be passed to the target method.</param>
        /// <returns>The method return value.</returns>
        public static object Invoke<T1, T2, T3, T4>(this object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
                throw new NullReferenceException("instance");

            var typeArguments = new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
            return Invoke(instance, methodName, typeArguments, arguments);
        }

        /// <summary>
        /// Invokes a method on the target <paramref name="instance"/> using the given <paramref name="methodName"/>.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <param name="typeArguments">The type arguments that will be passed to the target method.</param>
        /// <param name="arguments">The arguments that will be passed to the target method.</param>
        /// <returns>The method return value.</returns>
        public static object Invoke(this object instance, string methodName, Type[] typeArguments, params object[] arguments)
        {
            var context = new MethodFinderContext(typeArguments, arguments, null);
            return Invoke(instance, methodName, context);
        }
    }
}
