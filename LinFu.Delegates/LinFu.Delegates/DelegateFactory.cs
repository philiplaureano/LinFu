using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Delegates
{
    public static class DelegateFactory
    {
        private static readonly ProxyFactory _factory = new ProxyFactory();
        private static readonly Dictionary<DelegateInfo, Type>
            _typeCache = new Dictionary<DelegateInfo, Type>();
        public static Type DefineDelegateType(string typeName, 
            Type returnType, ParameterInfo[] parameters)
        {
            List<Type> parameterTypes = new List<Type>();
            if (parameters != null)
            {
                foreach (ParameterInfo param in parameters)
                {
                    parameterTypes.Add(param.ParameterType);
                    continue;
                }
            }
            return DefineDelegateType(typeName, returnType, parameterTypes.ToArray());
        }
        public static Type DefineDelegateType(string typeName, Type returnType, params Type[] parameterTypes)
        {
            DelegateInfo info = new DelegateInfo(returnType, parameterTypes);

            if (_typeCache.ContainsKey(info))
                return _typeCache[info];

            AppDomain currentDomain = AppDomain.CurrentDomain;
            string assemblyName = string.Format("{0}Assembly", typeName);
            string moduleName = string.Format("{0}Module", typeName);

            AssemblyName name = new AssemblyName(assemblyName);
            AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
            AssemblyBuilder assemblyBuilder = currentDomain.DefineDynamicAssembly(name, access);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", moduleName), true);
            TypeAttributes typeAttributes = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.AnsiClass | TypeAttributes.AutoClass;
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, typeof(MulticastDelegate));

            // Delegate constructor
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.RTSpecialName |
                MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new Type[] { typeof(object), typeof(System.IntPtr) });

            constructorBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            // Define the Invoke method with a signature that matches the parameter types
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Invoke", methodAttributes, returnType, parameterTypes);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            #region Define the Begin/EndInvoke methods for async callback support
            methodBuilder = typeBuilder.DefineMethod("BeginInvoke", methodAttributes, typeof(System.IAsyncResult),
                new Type[] { typeof(int), typeof(AsyncCallback), typeof(object) });
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            methodBuilder = typeBuilder.DefineMethod("EndInvoke", methodAttributes, typeof(void),
                new Type[] { typeof(IAsyncResult) });
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
            #endregion

            Type result = typeBuilder.CreateType();

            // Cache the result
            if (result != null)
                _typeCache[info] = result;

            return result;
        }

        public static MulticastDelegate DefineDelegate(Type delegateType, CustomDelegate methodBody)
        {
            MethodInfo invokeMethod = delegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            
            Type returnType = invokeMethod.ReturnType;
            List<Type> parameterTypes = new List<Type>();
            foreach(ParameterInfo param in invokeMethod.GetParameters())
            {
                parameterTypes.Add(param.ParameterType);
            }
            return DefineDelegate(delegateType, methodBody, returnType, parameterTypes.ToArray());
        }
        public static MulticastDelegate DefineDelegate(CustomDelegate methodBody, Type returnType, params Type[] parameterTypes)
        {
            Type delegateType = DelegateFactory.DefineDelegateType("___Anonymous", returnType, parameterTypes);
            return DefineDelegate(delegateType, methodBody, returnType, parameterTypes);
        }
        public static MulticastDelegate DefineDelegate(Type delegateType, CustomDelegate methodBody, Type returnType, Type[] parameterTypes)
        {
            // Generate an interface that matches the given signature and return type
            Type interfaceType = InterfaceBuilder.DefineInterfaceMethod(returnType, parameterTypes);

            // Proxy the interface type 
            CustomDelegateRedirector redirector = new CustomDelegateRedirector(methodBody);
            object interfaceInstance = _factory.CreateProxy(typeof (object), redirector, interfaceType);

            // Map the call from the custom delegate to the target
            // delegate type
            MethodInfo targetMethod = interfaceType.GetMethods()[0];
            MulticastDelegate result = BindTo(delegateType, targetMethod, interfaceInstance);


            return result;
        }

        private static MulticastDelegate BindTo(Type delegateType, 
            MethodInfo targetMethod, object interfaceInstance)
        {
            IntPtr methodPointer = targetMethod.MethodHandle.GetFunctionPointer();

            // Attach the newly implemented interface to the target delegate

            // Generate the custom delegate type 
            MulticastDelegate result = null;

            try
            {
                result = (MulticastDelegate)Activator.CreateInstance(delegateType, new object[] { interfaceInstance, methodPointer });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            return result;
        }

        public static MulticastDelegate DefineDelegate(object instance, MethodInfo targetMethod)
        {
            Type delegateType = DefineDelegateType("Anonymous", targetMethod.ReturnType, targetMethod.GetParameters());

            return BindTo(delegateType, targetMethod, instance);
        }
    }
}
