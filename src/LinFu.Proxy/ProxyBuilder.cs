using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Interfaces;
using LinFu.Proxy.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit.Interfaces;
using Mono.Cecil;

namespace LinFu.Proxy
{
    /// <summary>
    /// A <see cref="IProxyBuilder"/> type that generates
    /// proxies that forward all virtual method calls to a 
    /// single interceptor.
    /// </summary>
    [Implements(typeof(IProxyBuilder), LifecycleType.OncePerRequest)]
    internal class ProxyBuilder : IProxyBuilder, IInitialize
    {
        /// <summary>
        /// Initializes the current class with the default values.
        /// </summary>
        public ProxyBuilder()
        {
            ProxyImplementor = new ProxyImplementor();
            MethodPicker = new MethodPicker();
            ProxyMethodBuilder = new ProxyMethodBuilder();
        }

        /// <summary>
        /// Generates a proxy that forwards all virtual method calls
        /// to a single <see cref="IInterceptor"/> instance.
        /// </summary>
        /// <param name="originalBaseType">The base class of the type being constructed.</param>
        /// <param name="baseInterfaces">The list of interfaces that the new type must implement.</param>
        /// <param name="module">The module that will hold the brand new type.</param>
        /// <param name="targetType">The <see cref="TypeDefinition"/> that represents the type to be created.</param>
        public void Construct(Type originalBaseType, IEnumerable<Type> baseInterfaces,
                                          ModuleDefinition module, TypeDefinition targetType)
        {
            // Determine which interfaces need to be implemented
            var interfaces = new HashSet<Type>(baseInterfaces);

            // Implement the IProxy interface
            if (ProxyImplementor != null)
                ProxyImplementor.Construct(module, targetType);

            // Determine which methods should be proxied
            IEnumerable<MethodInfo> targetMethods = new MethodInfo[0];
            if (MethodPicker != null)
                targetMethods = MethodPicker.ChooseProxyMethodsFrom(originalBaseType, interfaces);

            if (ProxyMethodBuilder == null)
                return;
            
            // Generate a proxy method for each
            // target method
            foreach (var method in targetMethods)
            {
                ProxyMethodBuilder.CreateMethod(targetType, method);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ITypeBuilder"/> interface
        /// which will emit the actual implementation of the IProxy interface.
        /// </summary>
        public ITypeBuilder ProxyImplementor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMethodPicker"/>
        /// instance that will determine which methods
        /// will be proxied by the proxy builder.
        /// </summary>
        public IMethodPicker MethodPicker { get; set; }

        /// <summary>
        /// The <see cref="IMethodBuilder"/> instance
        /// that will be responsible for generating each method
        /// for the current target type.
        /// </summary>
        public IMethodBuilder ProxyMethodBuilder
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the current instance
        /// with the <paramref name="source"/> container.
        /// </summary>
        /// <param name="source">The <see cref="IServiceContainer"/> instance that will hold the current instance.</param>
        public void Initialize(IServiceContainer source)
        {
            ProxyImplementor = source.GetService<ITypeBuilder>("ProxyImplementor");
            MethodPicker = source.GetService<IMethodPicker>();
            ProxyMethodBuilder = source.GetService<IMethodBuilder>("ProxyMethodBuilder");
        }
    }
}