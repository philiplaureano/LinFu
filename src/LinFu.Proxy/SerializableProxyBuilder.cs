using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using System.Runtime.Serialization;
using Mono.Cecil.Cil;
using System.Reflection;
using LinFu.Proxy.Interfaces;

using MethodAttributes = Mono.Cecil.MethodAttributes;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration;
namespace LinFu.Proxy
{
    /// <summary>
    /// Represents a <see cref="ProxyBuilder"/> type that can create serializable proxy types.
    /// </summary>
    [Implements(typeof(IProxyBuilder), LifecycleType.OncePerRequest)]
    public class SerializableProxyBuilder : ProxyBuilder
    {
        /// <summary>
        /// Generates a proxy that forwards all virtual method calls
        /// to a single <see cref="IInterceptor"/> instance.
        /// </summary>
        /// <param name="originalBaseType">The base class of the type being constructed.</param>
        /// <param name="baseInterfaces">The list of interfaces that the new type must implement.</param>
        /// <param name="module">The module that will hold the brand new type.</param>
        /// <param name="targetType">The <see cref="TypeDefinition"/> that represents the type to be created.</param>
        public override void Construct(Type originalBaseType, IEnumerable<Type> baseInterfaces, ModuleDefinition module, Mono.Cecil.TypeDefinition targetType)
        {
            var interfaces = new HashSet<Type>(baseInterfaces);

            if (!interfaces.Contains(typeof(ISerializable)))
                interfaces.Add(typeof(ISerializable));

            var serializableInterfaceType = module.ImportType<ISerializable>();
            if (!targetType.Interfaces.Contains(serializableInterfaceType))
                targetType.Interfaces.Add(serializableInterfaceType);

            // Create the proxy type
            base.Construct(originalBaseType, interfaces, module, targetType);

            // Add the Serializable attribute
            targetType.IsSerializable = true;

            var serializableCtor = module.ImportConstructor<SerializableAttribute>();
            var serializableAttribute = new CustomAttribute(serializableCtor);
            targetType.CustomAttributes.Add(serializableAttribute);
            
            ImplementGetObjectData(originalBaseType, baseInterfaces, module, targetType);
            DefineSerializationConstructor(module, targetType);

            var interceptorType = module.ImportType<IInterceptor>();
            var interceptorGetterProperty = (from PropertyDefinition m in targetType.Properties
                                          where m.Name == "Interceptor" && m.PropertyType == interceptorType
                                          select m).First();
        }

        private static void DefineSerializationConstructor(ModuleDefinition module, Mono.Cecil.TypeDefinition targetType)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);

            Type[] parameterTypes = new Type[] { typeof(SerializationInfo), typeof(StreamingContext) };
            
            // Define the constructor signature
            var serializationCtor = targetType.AddDefaultConstructor();
            serializationCtor.AddParameters(parameterTypes);

            serializationCtor.Attributes = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Public;
            var interceptorInterfaceType = module.ImportType<IInterceptor>();
            var interceptorTypeVariable = serializationCtor.AddLocal<Type>();

            var body = serializationCtor.Body;
            body.InitLocals = true;

            var IL = serializationCtor.GetILGenerator();
            
            IL.Emit(OpCodes.Ldtoken, interceptorInterfaceType);
            IL.Emit(OpCodes.Call, getTypeFromHandle);
            IL.Emit(OpCodes.Stloc, interceptorTypeVariable);

            var defaultConstructor = module.ImportConstructor<object>();
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Call, defaultConstructor);

            // __interceptor = (IInterceptor)info.GetValue("__interceptor", typeof(IInterceptor));
            var getValue = module.ImportMethod<SerializationInfo>("GetValue");
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldstr, "__interceptor");
            IL.Emit(OpCodes.Ldloc, interceptorTypeVariable);
            IL.Emit(OpCodes.Callvirt, getValue);
            IL.Emit(OpCodes.Castclass, interceptorInterfaceType);

            var setInterceptor = module.ImportMethod<IProxy>("set_Interceptor");
            IL.Emit(OpCodes.Callvirt, setInterceptor); ;
            IL.Emit(OpCodes.Ret);
        }

        private static void ImplementGetObjectData(Type originalBaseType, IEnumerable<Type> baseInterfaces, ModuleDefinition module, Mono.Cecil.TypeDefinition targetType)
        {
            var getObjectDataMethod = (from MethodDefinition m in targetType.Methods
                                       where m.Name.Contains("ISerializable.GetObjectData")
                                       select m).First();

            var body = getObjectDataMethod.Body;
            body.Instructions.Clear();
            body.InitLocals = true;

            var IL = getObjectDataMethod.GetILGenerator();

            var proxyInterfaceType = module.ImportType<IProxy>();
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            var proxyObjectRefType = module.ImportType<ProxyObjectReference>();
            var setType = module.ImportMethod<SerializationInfo>("SetType", BindingFlags.Public | BindingFlags.Instance);
            var getInterceptor = module.ImportMethod<IProxy>("get_Interceptor");

            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldtoken, proxyObjectRefType);
            IL.Emit(OpCodes.Call, getTypeFromHandle);
            IL.Emit(OpCodes.Callvirt, setType);

            // info.AddValue("__interceptor", __interceptor);
            var addValueMethod = typeof(SerializationInfo).GetMethod("AddValue", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(object) }, null);
            var addValue = module.Import(addValueMethod);

            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldstr, "__interceptor");
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Castclass, proxyInterfaceType);
            IL.Emit(OpCodes.Callvirt, getInterceptor);
            IL.Emit(OpCodes.Callvirt, addValue);

            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldstr, "__baseType");
            IL.Emit(OpCodes.Ldstr, originalBaseType.AssemblyQualifiedName);
            IL.Emit(OpCodes.Callvirt, addValue);

            int baseInterfaceCount = baseInterfaces.Count();

            // Save the number of base interfaces
            var integerType = module.ImportType<Int32>();
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Ldstr, "__baseInterfaceCount");
            IL.Emit(OpCodes.Ldc_I4, baseInterfaceCount);
            IL.Emit(OpCodes.Box, integerType);
            IL.Emit(OpCodes.Callvirt, addValue);

            int index = 0;
            foreach (Type baseInterface in baseInterfaces)
            {
                IL.Emit(OpCodes.Ldarg_1);
                IL.Emit(OpCodes.Ldstr, string.Format("__baseInterface{0}", index++));
                IL.Emit(OpCodes.Ldstr, baseInterface.AssemblyQualifiedName);
                IL.Emit(OpCodes.Callvirt, addValue);
            }

            IL.Emit(OpCodes.Ret);
        }
    }
}
