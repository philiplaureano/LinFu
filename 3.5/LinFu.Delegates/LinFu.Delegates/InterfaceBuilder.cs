using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace LinFu.Delegates
{
    public class InterfaceBuilder
    {
        private string _interfaceName;
        private List<InterfaceMethodInfo> _methods = new List<InterfaceMethodInfo>();
        private static readonly Dictionary<InterfaceInfo, Type> _cache = new Dictionary<InterfaceInfo, Type>();
        public InterfaceBuilder(string interfaceName)
        {
            InterfaceName = interfaceName;
        }
                
        public List<InterfaceMethodInfo> Methods
        {
            get { return _methods; }
        }

        public string InterfaceName
        {
            get { return _interfaceName; }
            set { _interfaceName = value; }
        }
        public Type CreateInterface()
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = Guid.NewGuid().ToString();

            string moduleName = assemblyName.Name;
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", moduleName), true);

            TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Interface |
                                        TypeAttributes.AutoClass | TypeAttributes.AnsiClass
                                        | TypeAttributes.Abstract;

            TypeBuilder typeBuilder = moduleBuilder.DefineType(_interfaceName, attributes);

            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.NewSlot
                                                | MethodAttributes.HideBySig | MethodAttributes.Virtual |
                                                MethodAttributes.Abstract;

            foreach (InterfaceMethodInfo info in _methods)
            {
                MethodBuilder method = typeBuilder.DefineMethod(info.MethodName, methodAttributes, info.ReturnType, info.ArgumentTypes);
                method.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);
            }

            return typeBuilder.CreateType();
        }
        public void AddMethod(string methodName, Type returnType, Type[] parameters)
        {
            InterfaceMethodInfo info = new InterfaceMethodInfo();
            info.MethodName = methodName;
            info.ReturnType = returnType;
            info.ArgumentTypes = parameters;

            _methods.Add(info);
        }
        public static Type DefineInterfaceMethod(Type returnType, Type[] parameters)
        {
            // Reuse the previously cached results
            InterfaceInfo cacheKey = new InterfaceInfo(returnType, parameters);
            if (_cache.ContainsKey(cacheKey))
                return _cache[cacheKey];

            Type result = CreateInterface(returnType, parameters);

            // Cache the results
            if (result != null)
                _cache[cacheKey] = result;

            return result;
        }

        private static Type CreateInterface(Type returnType, Type[] parameters)
        {
            string interfaceName = "IAnonymous";
            string methodName = "AnonymousMethod";
            return CreateInterface(interfaceName, methodName, returnType, parameters);
        }

        private static Type CreateInterface(string interfaceName, string methodName, Type returnType, Type[] parameters)
        {
            InterfaceBuilder builder = new InterfaceBuilder(interfaceName);
            builder.AddMethod(methodName, returnType, parameters);

            return builder.CreateInterface();
        }        
    }
}
