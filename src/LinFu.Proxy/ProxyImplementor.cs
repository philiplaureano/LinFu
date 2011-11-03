using System.Linq;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.Proxy.Interfaces;
using LinFu.Reflection.Emit;
using LinFu.Reflection.Emit.Interfaces;
using Mono.Cecil;
using CallingConventions = Mono.Cecil.MethodCallingConvention;

namespace LinFu.Proxy
{
    /// <summary>
    /// A class that provides the default implementation
    /// for the IProxy interface.
    /// </summary>
    [Implements(typeof (ITypeBuilder), LifecycleType.OncePerRequest, ServiceName = "ProxyImplementor")]
    internal class ProxyImplementor : ITypeBuilder
    {
        #region ITypeBuilder Members

        /// <summary>
        /// Constructs a type that implements the
        /// <see cref="IProxy"/> interface.
        /// </summary>
        /// <param name="module">The module that will hold the target type.</param>
        /// <param name="targetType">The type that will implement the <see cref="IProxy"/> interface.</param>
        public void Construct(ModuleDefinition module, TypeDefinition targetType)
        {
            TypeReference proxyInterfaceType = module.Import(typeof (IProxy));
            TypeReference interceptorType = module.Import(typeof (IInterceptor));

            // Implement the IProxy interface only once
            if (targetType.Interfaces.Any(typeReference => typeReference.FullName == proxyInterfaceType.FullName))
                return;

            targetType.Interfaces.Add(proxyInterfaceType);
            targetType.AddProperty("Interceptor", interceptorType);
        }

        #endregion
    }
}