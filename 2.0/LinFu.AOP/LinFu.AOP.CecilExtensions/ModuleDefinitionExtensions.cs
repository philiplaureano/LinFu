using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection;

namespace LinFu.AOP.CecilExtensions
{
    public static class ModuleDefinitionExtensions
    {
        public static MethodReference ImportConstructor<T>(this ModuleDefinition module, params Type[] constructorParameters)
        {
            return module.Import(typeof(T).GetConstructor(constructorParameters));
        }
        public static MethodReference ImportMethod(this ModuleDefinition module, string methodName, Type declaringType)
        {
            return module.Import(declaringType.GetMethod(methodName));
        }
        public static MethodReference ImportMethod(this ModuleDefinition module, string methodName, Type declaringType, BindingFlags flags)
        {
            return module.Import(declaringType.GetMethod(methodName, flags));
        }
        public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName)
        {
            return module.Import(typeof(T).GetMethod(methodName));
        }
        public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName, params Type[] parameterTypes)
        {
            return module.Import(typeof(T).GetMethod(methodName, parameterTypes));
        }
        public static MethodReference ImportMethod<T>(this ModuleDefinition module, 
            string methodName, BindingFlags flags)
        {
            return module.Import(typeof(T).GetMethod(methodName, flags));
        }
        public static TypeReference ImportType<T>(this ModuleDefinition module)
        {
            return module.Import(typeof(T));
        }
        public static TypeReference ImportType(this ModuleDefinition module, Type targetType)
        {
            return module.Import(targetType);
        }
    }
}
