using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Proxy.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace LinFu.Proxy
{
    /// <summary>
    /// Provides the basic implementation for a proxy factory class.
    /// </summary>
    [Implements(typeof(IProxyFactory), LifecycleType.OncePerRequest)]
    public class ProxyFactory : IProxyFactory, IInitialize
    {
        /// <summary>
        /// Initializes the proxy factory with the default values.
        /// </summary>
        public ProxyFactory()
        {
            // Use the forwarding proxy type by default
            ProxyBuilder = new SerializableProxyBuilder();
            InterfaceExtractor = new InterfaceExtractor();
            Cache = new ProxyCache();
        }
        #region IProxyFactory Members

        /// <summary>
        /// Creates a proxy type using the given
        /// <paramref name="baseType"/> as the base class
        /// and ensures that the proxy type implements the given
        /// interface types.
        /// </summary>
        /// <param name="baseType">The base class from which the proxy type will be derived.</param>
        /// <param name="baseInterfaces">The list of interfaces that the proxy will implement.</param>
        /// <returns>A forwarding proxy.</returns>
        public Type CreateProxyType(Type baseType, IEnumerable<Type> baseInterfaces)
        {

            // Reuse the cached results, if possible
            var originalInterfaces = baseInterfaces.ToArray();
            if (Cache != null && Cache.Contains(baseType, originalInterfaces))
                return Cache.Get(baseType, originalInterfaces);

            if (!baseType.IsPublic)
                throw new ArgumentException("The proxy factory can only generate proxies from public base classes.",
                                            "baseType");

            var hasNonPublicInterfaces = (from t in baseInterfaces
                                          where t.IsNotPublic
                                          select t).Count() > 0;

            if (hasNonPublicInterfaces)
                throw new ArgumentException("The proxy factory cannot generate proxies from non-public interfaces.",
                                            "baseInterfaces");

            #region Determine which interfaces need to be implemented
            var actualBaseType = baseType.IsInterface ? typeof(object) : baseType;
            var interfaces = new HashSet<Type>(baseInterfaces);
            // Move the base type into the list of interfaces
            // if the user mistakenly entered
            // an interface type as the base type
            if (baseType.IsInterface)
            {
                interfaces.Add(baseType);
            }

            if (InterfaceExtractor != null)
            {
                // Get the interfaces for the base type
                InterfaceExtractor.GetInterfaces(actualBaseType, interfaces);

                var targetList = interfaces.ToArray();
                // Extract the inherited interfaces
                foreach (var type in targetList)
                {
                    InterfaceExtractor.GetInterfaces(type, interfaces);
                }
            }

            #endregion


            #region Generate the assembly

            var assemblyName = "LinFu.Proxy";
            var assembly = AssemblyFactory.DefineAssembly(assemblyName, AssemblyKind.Dll);
            var mainModule = assembly.MainModule;
            var importedBaseType = mainModule.Import(actualBaseType);
            var attributes = TypeAttributes.AutoClass | TypeAttributes.Class |
                                        TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

            #endregion

            #region Initialize the proxy type
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            var typeName = string.Format("{0}Proxy-{1}", baseType.Name, guid);
            var namespaceName = "LinFu.Proxy";
            var proxyType = mainModule.DefineClass(typeName, namespaceName,
                                                              attributes, importedBaseType);

            proxyType.AddDefaultConstructor();
            #endregion

            if (ProxyBuilder == null)
                throw new NullReferenceException("The 'ProxyBuilder' property cannot be null");

            // Add the list of interfaces to the target type
            foreach (var interfaceType in interfaces)
            {
                if (!interfaceType.IsInterface)
                    continue;

                var currentType = mainModule.Import(interfaceType);
                proxyType.Interfaces.Add(currentType);
            }

            // Hand it off to the builder for construction
            ProxyBuilder.Construct(actualBaseType, interfaces, mainModule, proxyType);

            // Verify the assembly, if possible
            if (Verifier != null)
                Verifier.Verify(assembly);

            #region Compile the results
            var compiledAssembly = assembly.ToAssembly();

            IEnumerable<Type> types = null;

            try
            {
                types = compiledAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            var result = (from t in types
                          where t != null && t.IsClass
                          select t).FirstOrDefault();
            #endregion

            // Cache the result
            if (Cache != null)
                Cache.Store(result, baseType, originalInterfaces);

            return result;
        }

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="IExtractInterfaces"/> type that will be
        /// responsible for determining which interfaces
        /// the proxy type should implement.
        /// </summary>
        public IExtractInterfaces InterfaceExtractor { get; set; }

        /// <summary>
        /// The <see cref="IProxyBuilder"/> instance that is
        /// responsible for generating the proxy type.
        /// </summary>
        public IProxyBuilder ProxyBuilder { get; set; }

        /// <summary>
        /// The <see cref="IVerifier"/> instance that will be used to 
        /// ensure that the generated assemblies are valid.
        /// </summary>
        public IVerifier Verifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the <see cref="IProxyCache"/>
        /// instance that will be used to cache previous proxy generation runs.
        /// </summary>
        public IProxyCache Cache { get; set; }

        /// <summary>
        /// Initializes the <see cref="ProxyFactory"/> instance
        /// with the <paramref name="source"/> container.
        /// </summary>
        /// <param name="source">The <see cref="IServiceContainer"/> instance that will hold the ProxyFactory.</param>
        public virtual void Initialize(IServiceContainer source)
        {
            if (source.Contains(typeof(IProxyBuilder), new Type[0]))
                ProxyBuilder = (IProxyBuilder)source.GetService(typeof(IProxyBuilder));

            if (source.Contains(typeof(IExtractInterfaces), new Type[0]))
                InterfaceExtractor = (IExtractInterfaces)source.GetService(typeof(IExtractInterfaces));

            //if (source.Contains(typeof(IVerifier)))
            //    Verifier = source.GetService<IVerifier>();

            if (source.Contains(typeof(IProxyCache), new Type[0]))
                Cache = (IProxyCache)source.GetService(typeof(IProxyCache));
        }
    }
}