using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System.Diagnostics;
using System.Reflection;
using LinFu.AOP.Interfaces;
using LinFu.AOP.CecilExtensions;

namespace LinFu.AOP.Weavers.Cecil
{
    public class InvocationContextBuilder
    {
        private HashSet<MethodDefinition> _wovenMethods = new HashSet<MethodDefinition>();
        public void BuildContext(CilWorker IL, MethodDefinition methodDef, VariableDefinition context,
            Queue<Instruction> instructions)
        {
            // The method should only be woven once
            if (_wovenMethods.Contains(methodDef))
                return;
          
            var module = methodDef.DeclaringType.Module;

            var currentMethod = methodDef.AddLocal(typeof(MethodBase));
            var parameterTypes = methodDef.AddLocal(typeof(Type[]));
            var arguments = methodDef.AddLocal(typeof(object[]));
            var typeArguments = methodDef.AddLocal(typeof(Type[]));
            
            var systemType = module.ImportType(typeof(Type));

            #region Initialize the InvocationInfo constructor arguments

            // Type[] typeArguments = new Type[genericTypeCount];
            int genericParameterCount = methodDef.GenericParameters.Count;
            instructions.Enqueue(IL.Create(OpCodes.Ldc_I4, genericParameterCount));
            instructions.Enqueue(IL.Create(OpCodes.Newarr, systemType));
            instructions.Enqueue(IL.Create(OpCodes.Stloc, typeArguments));

            // Push the generic type arguments onto the stack
            if (genericParameterCount > 0)
                IL.PushGenericArguments(methodDef, module, instructions, typeArguments,
                    genericParameterCount);

            // object[] arguments = new object[argumentCount];            
            IL.PushArguments(module, methodDef, arguments, instructions);

            // object target = this;
            IL.PushInstance(methodDef, instructions);
            IL.PushMethod(methodDef, module, instructions);

            instructions.Enqueue(IL.Create(OpCodes.Stloc, currentMethod));

            // MethodInfo targetMethod = currentMethod as MethodInfo;
            var methodInfoType = module.Import(typeof(MethodInfo));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, currentMethod));
            instructions.Enqueue(IL.Create(OpCodes.Isinst, methodInfoType));

            // Get the current stack trace
            IL.PushStackTrace(instructions, module);

            // Push the type arguments back onto the stack
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, typeArguments));

            // Save the parameter types
            instructions.Enqueue(IL.Create(OpCodes.Ldc_I4, methodDef.Parameters.Count));
            instructions.Enqueue(IL.Create(OpCodes.Newarr, systemType));
            instructions.Enqueue(IL.Create(OpCodes.Stloc, parameterTypes));
            IL.SaveParameterTypes(methodDef, module, parameterTypes, instructions);
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, parameterTypes));

            // Save the return type
            var getTypeFromHandle = module.Import(typeof(Type).GetMethod("GetTypeFromHandle",
                BindingFlags.Static | BindingFlags.Public));

            TypeReference returnType = methodDef.ReturnType.ReturnType;
            instructions.Enqueue(IL.Create(OpCodes.Ldtoken, returnType));
            instructions.Enqueue(IL.Create(OpCodes.Call, getTypeFromHandle));

            // Push the arguments back onto the stack
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, arguments));
            #endregion

            Type[] types = new Type[] { typeof(object), 
                                        typeof(MethodInfo), 
                                        typeof(StackTrace), 
                                        typeof(Type[]), 
                                        typeof(Type[]), 
                                        typeof(Type), 
                                        typeof(object[]) };

            // InvocationContext context = new InvocationContext(...);
            var contextCtor = module.ImportConstructor<InvocationContext>(types);
            instructions.Enqueue(IL.Create(OpCodes.Newobj, contextCtor));
            instructions.Enqueue(IL.Create(OpCodes.Stloc, context));

            _wovenMethods.Add(methodDef);           
        }
    }
}
