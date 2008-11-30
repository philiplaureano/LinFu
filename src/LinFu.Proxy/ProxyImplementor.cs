using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Proxy.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.Reflection.Emit.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using CallingConventions = Mono.Cecil.MethodCallingConvention;

namespace LinFu.Proxy
{
    /// <summary>
    /// A class that provides the default implementation
    /// for the IProxy interface.
    /// </summary>
    [Implements(typeof(ITypeBuilder), LifecycleType.OncePerRequest, ServiceName = "ProxyImplementor")]
    public class ProxyImplementor : ITypeBuilder
    {
        /// <summary>
        /// Constructs a type that implements the
        /// <see cref="IProxy"/> interface.
        /// </summary>
        /// <param name="module">The module that will hold the target type.</param>
        /// <param name="targetType">The type that will implement the <see cref="IProxy"/> interface.</param>
        public void Construct(ModuleDefinition module, TypeDefinition targetType)
        {
            var proxyInterfaceType = module.Import(typeof(IProxy));
            var interceptorType = module.Import(typeof(IInterceptor));

            // Implement the IProxy interface only once
            if (targetType.Interfaces.Contains(proxyInterfaceType))
                return;

            targetType.Interfaces.Add(proxyInterfaceType);
            targetType.AddProperty("Interceptor", interceptorType);
        }
    }
}
