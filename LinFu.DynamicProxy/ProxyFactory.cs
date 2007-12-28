using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    public class ProxyFactory
    {
        private static readonly ConstructorInfo baseConstructor = typeof (object).GetConstructor(new Type[0]);
        private IProxyCache _cache = new ProxyCache();
        private IProxyMethodBuilder _proxyMethodBuilder;

        public ProxyFactory() : this(new DefaultyProxyMethodBuilder())
        {
        }

        public ProxyFactory(IProxyMethodBuilder proxyMethodBuilder)
        {
            _proxyMethodBuilder = proxyMethodBuilder;
        }

        public IProxyCache Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        public IProxyMethodBuilder ProxyMethodBuilder
        {
            get { return _proxyMethodBuilder; }
            set { _proxyMethodBuilder = value; }
        }

        public virtual object CreateProxy(Type instanceType, IInvokeWrapper wrapper, params Type[] baseInterfaces)
        {
            return CreateProxy(instanceType, new CallAdapter(wrapper), baseInterfaces);
        }

        public virtual object CreateProxy(Type instanceType, IInterceptor interceptor, params Type[] baseInterfaces)
        {
            Type proxyType = CreateProxyType(instanceType, baseInterfaces);
            object result = Activator.CreateInstance(proxyType);
            IProxy proxy = (IProxy) result;
            proxy.Interceptor = interceptor;

            return result;
        }

        public virtual T CreateProxy<T>(IInvokeWrapper wrapper, params Type[] baseInterfaces)
        {
            return CreateProxy<T>(new CallAdapter(wrapper), baseInterfaces);
        }

        public virtual T CreateProxy<T>(IInterceptor interceptor, params Type[] baseInterfaces)
        {
            Type proxyType = CreateProxyType(typeof (T), baseInterfaces);
            T result = (T) Activator.CreateInstance(proxyType);
            Debug.Assert(result != null);

            IProxy proxy = (IProxy) result;
            proxy.Interceptor = interceptor;

            return result;
        }

        public virtual Type CreateProxyType(Type baseType, params Type[] baseInterfaces)
        {
            // Reuse the previous results, if possible
            if (_cache != null && _cache.Contains(baseType, baseInterfaces))
                return _cache.GetProxyType(baseType, baseInterfaces);

            Type result = CreateUncachedProxyType(baseInterfaces, baseType);

            // Cache the proxy type
            if (result != null && _cache != null)
                _cache.StoreProxyType(result, baseType, baseInterfaces);

            return result;
        }

        private Type CreateUncachedProxyType(Type[] baseInterfaces, Type baseType)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            string typeName = string.Format("{0}Proxy", baseType.Name);
            string assemblyName = string.Format("{0}Assembly", typeName);
            string moduleName = string.Format("{0}Module", typeName);
            ;

            AssemblyName name = new AssemblyName(assemblyName);

#if DEBUG
            AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
#else
            AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
#endif
            AssemblyBuilder assemblyBuilder = currentDomain.DefineDynamicAssembly(name, access);

#if DEBUG
            ModuleBuilder moduleBuilder =
                assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", moduleName), true);
#else
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif

            TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class |
                                            TypeAttributes.Public;

            List<Type> interfaceList = new List<Type>();
            if (baseInterfaces != null && baseInterfaces.Length > 0)
                interfaceList.AddRange(baseInterfaces);


            // Use the proxy dummy as the base type 
            // since we're not inheriting from any class type
            Type parentType = baseType;
            if (baseType.IsInterface)
            {
                parentType = typeof (ProxyDummy);
                interfaceList.Add(baseType);
            }

            // Add any inherited interfaces
            Type[] interfaces = interfaceList.ToArray();
            foreach (Type interfaceType in interfaces)
            {
                BuildInterfaceList(interfaceType, interfaceList);
            }

            TypeBuilder typeBuilder =
                moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaceList.ToArray());

            DefineConstructor(typeBuilder);

            // Implement IProxy
            ProxyImplementor implementor = new ProxyImplementor();
            implementor.ImplementProxy(typeBuilder);

            MethodInfo[] methods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<MethodInfo> proxyList = new List<MethodInfo>();
            BuildMethodList(interfaceList, methods, proxyList);

            Debug.Assert(_proxyMethodBuilder != null, "ProxyMethodBuilder cannot be null");
            proxyList.ForEach(
                delegate(MethodInfo method) { _proxyMethodBuilder.CreateProxiedMethod(implementor.InterceptorField, method, typeBuilder); });

            // Make the proxy serializable
            ConstructorInfo serializableConstructor = typeof(SerializableAttribute).GetConstructor(new Type[0]);
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, new object[0]);
            typeBuilder.SetCustomAttribute(customAttributeBuilder);

            Type proxyType = typeBuilder.CreateType();

#if DEBUG_PROXY_OUTPUT
            assemblyBuilder.Save("generatedAssembly.dll");
#endif
            return proxyType;
        }

        private void BuildInterfaceList(Type currentType, List<Type> interfaceList)
        {
            Type[] interfaces = currentType.GetInterfaces();
            if (interfaces == null || interfaces.Length == 0)
                return;

            foreach (Type current in interfaces)
            {
                if (interfaceList.Contains(current))
                    continue;

                interfaceList.Add(current);
                BuildInterfaceList(current, interfaceList);
            }
        }

        private static void BuildMethodList(IEnumerable<Type> interfaceList, IEnumerable<MethodInfo> methods,
                                            List<MethodInfo> proxyList)
        {
            foreach (MethodInfo method in methods)
            {
                //if (method.DeclaringType == typeof(object))
                //    continue;

                // Only non-private methods will be proxied
                if (method.IsPrivate)
                    continue;

                // Final methods cannot be overridden
                if (method.IsFinal)
                    continue;

                // Only virtual methods can be intercepted
                if (!method.IsVirtual && !method.IsAbstract)
                    continue;

                proxyList.Add(method);
            }

            foreach (Type interfaceType in interfaceList)
            {
                MethodInfo[] interfaceMethods = interfaceType.GetMethods();
                foreach (MethodInfo interfaceMethod in interfaceMethods)
                {
                    if (proxyList.Contains(interfaceMethod))
                        continue;

                    proxyList.Add(interfaceMethod);
                }
            }
        }


        private static void DefineConstructor(TypeBuilder typeBuilder)
        {
            MethodAttributes constructorAttributes = MethodAttributes.Public |
                                                     MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                                                     MethodAttributes.RTSpecialName;

            ConstructorBuilder constructor =
                typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, new Type[] {});

            ILGenerator IL = constructor.GetILGenerator();

            constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Call, baseConstructor);
            IL.Emit(OpCodes.Ret);
        }
    }
}