using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace LinFu.DynamicProxy
{
    public class ProxyFactory
    {
        private static readonly ConstructorInfo baseConstructor = typeof (object).GetConstructor(new Type[0]);
        private static readonly MethodInfo getTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
        private static readonly MethodInfo getValue = typeof(SerializationInfo).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(Type) }, null);
        private static readonly MethodInfo setType = typeof(SerializationInfo).GetMethod("SetType", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null);
        private static readonly MethodInfo addValue = typeof(SerializationInfo).GetMethod("AddValue", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(object) }, null);
        private static readonly MethodInfo getQualifiedName = typeof(Type).GetMethod("get_AssemblyQualifiedName");

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
                                            TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

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

            // Add the ISerializable interface so that it can be implemented
            if (!interfaceList.Contains(typeof(ISerializable)))
                interfaceList.Add(typeof(ISerializable));

            TypeBuilder typeBuilder =
                moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaceList.ToArray());

            ConstructorBuilder defaultConstructor = DefineConstructor(typeBuilder);

            // Implement IProxy
            ProxyImplementor implementor = new ProxyImplementor();
            implementor.ImplementProxy(typeBuilder);

            MethodInfo[] methods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<MethodInfo> proxyList = new List<MethodInfo>();
            BuildMethodList(interfaceList, methods, proxyList);


            Debug.Assert(_proxyMethodBuilder != null, "ProxyMethodBuilder cannot be null");

            FieldInfo interceptorField = implementor.InterceptorField;
            foreach(MethodInfo method in proxyList)
            {
                // Provide a custom implementation of ISerializable
                // instead of redirecting it back to the interceptor
                if (method.DeclaringType == typeof(ISerializable))
                    continue;

                _proxyMethodBuilder.CreateProxiedMethod(interceptorField, method, typeBuilder); 
            }

            // Make the proxy serializable
            AddSerializationSupport(baseType, typeBuilder, interceptorField, defaultConstructor);

            Type proxyType = typeBuilder.CreateType();

#if DEBUG_PROXY_OUTPUT
            assemblyBuilder.Save("generatedAssembly.dll");
#endif
            return proxyType;
        }

        private static void AddSerializationSupport(Type baseType, TypeBuilder typeBuilder, FieldInfo interceptorField, ConstructorBuilder defaultConstructor)
        {
            ConstructorInfo serializableConstructor = typeof(SerializableAttribute).GetConstructor(new Type[0]);
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, new object[0]);
            typeBuilder.SetCustomAttribute(customAttributeBuilder);

            DefineSerializationConstructor(typeBuilder, interceptorField, defaultConstructor);
            ImplementGetObjectData(baseType, typeBuilder, interceptorField);
        }

        private static void BuildInterfaceList(Type currentType, List<Type> interfaceList)
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
        private static void ImplementGetObjectData(Type baseType, TypeBuilder typeBuilder, FieldInfo interceptorField)
        {
            MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                          MethodAttributes.Virtual;
            Type[] parameterTypes = new Type[] { typeof(SerializationInfo), typeof(StreamingContext) };

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod("GetObjectData", attributes, typeof (void), parameterTypes);

            ILGenerator IL = methodBuilder.GetILGenerator();
            LocalBuilder proxyBaseType = IL.DeclareLocal(typeof (Type));

            // Type baseType = proxyType.BaseType;
            IL.Emit(OpCodes.Ldtoken, baseType);
            IL.Emit(OpCodes.Call, getTypeFromHandle);
            IL.Emit(OpCodes.Stloc, proxyBaseType);

            // info.SetType(typeof(ProxyObjectReference));
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldtoken, typeof(ProxyObjectReference));
            IL.Emit(OpCodes.Call, getTypeFromHandle);
            IL.Emit(OpCodes.Callvirt, setType);

            // info.AddValue("__interceptor", __interceptor);
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldstr, "__interceptor");
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldfld, interceptorField);
            IL.Emit(OpCodes.Callvirt, addValue);

            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldstr, "__baseType");
            IL.Emit(OpCodes.Ldloc, proxyBaseType);
            IL.Emit(OpCodes.Callvirt, getQualifiedName);
            IL.Emit(OpCodes.Callvirt, addValue);
            IL.Emit(OpCodes.Ret);
        }

        private static void DefineSerializationConstructor(TypeBuilder typeBuilder, FieldInfo interceptorField, ConstructorBuilder defaultConstructor)
        {
            MethodAttributes constructorAttributes = MethodAttributes.Public |
                                         MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                                         MethodAttributes.RTSpecialName;

            Type[] parameterTypes = new Type[] {typeof (SerializationInfo), typeof (StreamingContext)};
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(constructorAttributes, 
                CallingConventions.Standard, parameterTypes);

            ILGenerator IL = constructor.GetILGenerator();

            LocalBuilder interceptorType = IL.DeclareLocal(typeof(Type));
            //LocalBuilder interceptor = IL.DeclareLocal(typeof(IInterceptor));

            constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

            

            IL.Emit(OpCodes.Ldtoken, typeof(IInterceptor));
            IL.Emit(OpCodes.Call, getTypeFromHandle);
            IL.Emit(OpCodes.Stloc, interceptorType);

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Call, defaultConstructor);

            // __interceptor = (IInterceptor)info.GetValue("__interceptor", typeof(IInterceptor));
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldstr, "__interceptor");
            IL.Emit(OpCodes.Ldloc, interceptorType);
            IL.Emit(OpCodes.Callvirt, getValue);
            IL.Emit(OpCodes.Castclass, typeof(IInterceptor));
            IL.Emit(OpCodes.Stfld, interceptorField);

            IL.Emit(OpCodes.Ret);
        }

        private static ConstructorBuilder DefineConstructor(TypeBuilder typeBuilder)
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

            return constructor;
        }
    }
}