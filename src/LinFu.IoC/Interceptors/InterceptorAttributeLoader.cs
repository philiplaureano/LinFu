using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.AOP;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
using LinFu.Proxy.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// The class responsible for loading interceptors marked with the
    /// <see cref="InterceptsAttribute"/> class.
    /// </summary>
    internal class InterceptorAttributeLoader : ITypeLoader
    {
        private readonly ILoader<IServiceContainer> _loaderHost;

        /// <summary>
        /// Initializes the class with the given <paramref name="loaderHost"/>.
        /// </summary>
        /// <param name="loaderHost">The <see cref="ILoader{TTarget}"/> instance that will be responsible for loading the <see cref="IServiceContainer"/> instance itself.</param>
        internal InterceptorAttributeLoader(ILoader<IServiceContainer> loaderHost)
        {
            _loaderHost = loaderHost;
        }

        /// <summary>
        /// Loads an <see cref="IInterceptor"/> derived class into a particular <see cref="IServiceContainer"/> instance
        /// so that the current interceptor type can intercept calls made to services created from the given
        /// target container.
        /// </summary>
        /// <param name="input">The interceptor type.</param>
        /// <returns>By default, this will always return an empty set of container actions. The actual interceptor itself will be injected at the end of the postprocessor chain.</returns>
        public IEnumerable<Action<IServiceContainer>> Load(Type input)
        {
            object typeInstance = Activator.CreateInstance(input);
            var interceptor = typeInstance as IInterceptor;

            Func<IServiceRequestResult, IInterceptor> getInterceptor = null;

            // Return the interceptor by default
            if (interceptor != null)
            {
                getInterceptor = result =>
                                     {
                                         var target = result.ActualResult;
                                         var container = result.Container;
                                         var methodInvoke = container.GetService<IMethodInvoke<MethodInfo>>();
                                         var factory = container.GetService<IProxyFactory>();

                                         // Manually initialize the interceptor
                                         var initialize = interceptor as IInitialize;
                                         if (initialize != null)
                                             initialize.Initialize(container);

                                         return new Redirector(() => target, interceptor, factory, methodInvoke);
                                     };
            }

            if (typeInstance != null && typeInstance is IAroundInvoke)
            {
                // Convert the IAroundInvoke instance into 
                // a running interceptor
                var aroundInvoke = typeInstance as IAroundInvoke;
                getInterceptor = result =>
                                     {
                                         var container = result.Container;
                                         var methodInvoke = container.GetService<IMethodInvoke<MethodInfo>>();
                                         var factory = container.GetService<IProxyFactory>();

                                         // Manually initialize the interceptor
                                         var initialize = aroundInvoke as IInitialize;
                                         if (initialize != null)
                                             initialize.Initialize(container);

                                         // HACK: The adapter can't be created until runtime since
                                         // the aroundInvoke instance needs an actual target
                                         var target = result.ActualResult;
                                         var adapter = new AroundInvokeAdapter(() => target,
                                             methodInvoke, aroundInvoke);

                                         var redirector = new Redirector(() => target, adapter, factory, methodInvoke);

                                         return redirector;
                                     };
            }

            // The type must implement either the IInterceptor interface
            // or the IAroundInvoke interface
            if (getInterceptor == null)
                return new Action<IServiceContainer>[0];

            // Determine which service types should be intercepted
            var attributes = from attribute in input.GetCustomAttributes(typeof(InterceptsAttribute), false)
                             let currentAttribute = attribute as InterceptsAttribute
                             where currentAttribute != null
                             select currentAttribute;

            var interceptedTypes = new Dictionary<Type, HashSet<string>>();
            foreach (var attribute in attributes)
            {

                var serviceName = attribute.ServiceName;
                var serviceType = attribute.TargetType;

                // Keep track of the service name and service type
                // and mark the current type for interception
                // using the current interceptor
                if (!interceptedTypes.ContainsKey(serviceType))
                    interceptedTypes[serviceType] = new HashSet<string>();

                if (!interceptedTypes[serviceType].Contains(serviceName))
                    interceptedTypes[serviceType].Add(serviceName);
            }

            // There must be at least one InterceptsAttribute defined on
            // the input type
            if (interceptedTypes.Count == 0)
                return new Action<IServiceContainer>[0];

            // Match the service type with the current type
            Func<IServiceRequestResult, bool> filter = request =>
                {
                    var container = request.Container;

                    // There must be a valid proxy factory
                    if (container == null || !container.Contains(typeof(IProxyFactory)))
                        return false;

                    var serviceType = request.ServiceType;
                    // Ignore requests to intercept IMethodInvoke<MethodInfo>
                    if (serviceType == typeof(IMethodInvoke<MethodInfo>))
                        return false;

                    // Sealed types cannot be proxied by default
                    if (serviceType.IsSealed)
                        return false;

                    // Match any service name if the service name is blank
                    if (request.ServiceName == null && interceptedTypes.ContainsKey(serviceType))
                        return true;

                    // Match the service name and type
                    if (interceptedTypes.ContainsKey(serviceType) &&
                        interceptedTypes[serviceType].Contains(request.ServiceName))
                        return true;

                    if (!serviceType.IsGenericType)
                        return false;

                    // Determine if an interceptor can intercept the
                    // entire family of generic types
                    var baseDefinition = serviceType.GetGenericTypeDefinition();

                    // The list of intercepted types should contain
                    // the generic type definition and its matching 
                    // service name
                    var serviceName = request.ServiceName;

                    return interceptedTypes.ContainsKey(baseDefinition) && interceptedTypes[baseDefinition].Contains(serviceName);
                };

            // Create the proxy using the service request
            Func<IServiceRequestResult, object> createProxy = request => CreateProxyFrom(request, getInterceptor);

            // Place the interceptor at the end of the 
            // postprocessor chain
            var injector = new ProxyInjector(filter, createProxy);
            _loaderHost.Plugins.Add(new ProxyContainerPlugin(injector));

            return new Action<IServiceContainer>[0];
        }

        /// <summary>
        /// Generates a proxy instance from an existing <see cref="IServiceRequestResult"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="IServiceRequestResult"/> instance that describes the proxy type that must be generated.</param>
        /// <param name="getInterceptor">The <see cref="IInterceptor"/> functor that will create the interceptor which will handle all calls made to the proxy instance.</param>
        /// <returns
        /// >A service proxy.</returns>
        private static object CreateProxyFrom(IServiceRequestResult request, Func<IServiceRequestResult, IInterceptor> getInterceptor)
        {
            var interceptor = getInterceptor(request);

            var container = request.Container;
            var proxyFactory =
                container.GetService<IProxyFactory>();

            // The proxy factory must exist
            if (proxyFactory == null)
                return null;

            // Generate the proxy type
            var proxyType = proxyFactory.CreateProxyType(request.ServiceType,
                                                         new Type[0]);

            // The generated proxy instance
            // must implement IProxy
            var proxy = Activator.CreateInstance(proxyType) as IProxy;

            // Assign the interceptor
            if (proxy != null)
                proxy.Interceptor = interceptor;

            return proxy;
        }

        /// <summary>
        /// Determines whether or not a target type is an interceptor.
        /// </summary>
        /// <param name="inputType">The target type currently being tested.</param>
        /// <returns>Returns <c>true</c> if the <paramref name="inputType"/> is an interceptor; otherwise, it will return <c>false</c>.</returns>
        public bool CanLoad(Type inputType)
        {
            var attributes = inputType.GetCustomAttributes(typeof(InterceptsAttribute), false);

            if (attributes == null)
                attributes = new object[0];

            // The target type must have at least one InterceptsAttribute defined
            var matches = from attribute in attributes
                          let currentAttribute = attribute as InterceptsAttribute
                          where currentAttribute != null
                          select currentAttribute;

            return matches.Count() > 0;
        }
    }
}
