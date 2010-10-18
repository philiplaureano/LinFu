using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodDefinitionExtensions = LinFu.AOP.Cecil.Extensions.MethodDefinitionExtensions;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a class that emits the instructions that call the method replacement instead of the original method body.
    /// </summary>
    public class InvokeMethodReplacement : IInstructionEmitter
    {
        private readonly VariableDefinition _classMethodReplacementProvider;
        private readonly Instruction _executeOriginalInstructions;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _methodReplacementProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeMethodReplacement"/> class.
        /// </summary>
        /// <param name="executeOriginalInstructions">The instruction label that will be used if the original instructions should be executed.</param>
        /// <param name="methodReplacementProvider">The variable that contains the <see cref="IMethodReplacementProvider"/> instance.</param>
        /// <param name="classMethodReplacementProvider">The variable that contains the class-level <see cref="IMethodReplacementProvider"/> instance.</param>
        /// <param name="invocationInfo">The variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        public InvokeMethodReplacement(Instruction executeOriginalInstructions,
                                       VariableDefinition methodReplacementProvider,
                                       VariableDefinition classMethodReplacementProvider,
                                       VariableDefinition invocationInfo)
        {
            _executeOriginalInstructions = executeOriginalInstructions;
            _methodReplacementProvider = methodReplacementProvider;
            _classMethodReplacementProvider = classMethodReplacementProvider;
            _invocationInfo = invocationInfo;
        }

        #region IInstructionEmitter Members

        /// <summary>
        /// Emits the instructions that call the method replacement instead of the original method body.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that points to the current method body.</param>
        public void Emit(CilWorker IL)
        {
            ModuleDefinition module = IL.GetModule();
            MethodDefinition method = IL.GetMethod();
            TypeReference returnType = method.ReturnType.ReturnType;
            VariableDefinition methodReplacement = MethodDefinitionExtensions.AddLocal(method, typeof (IInterceptor));

            GetMethodReplacementInstance(method, IL, methodReplacement, _methodReplacementProvider, _invocationInfo);

            Instruction skipGetClassMethodReplacement = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brtrue, skipGetClassMethodReplacement);

            GetMethodReplacementInstance(method, IL, methodReplacement, _classMethodReplacementProvider, _invocationInfo);

            IL.Append(skipGetClassMethodReplacement);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brfalse, _executeOriginalInstructions);

            // var returnValue = replacement.Intercept(info);
            InvokeInterceptor(module, IL, methodReplacement, returnType, _invocationInfo);
        }

        #endregion

        private static void InvokeInterceptor(ModuleDefinition module, CilWorker IL,
                                              VariableDefinition methodReplacement, TypeReference returnType,
                                              VariableDefinition invocationInfo)
        {
            MethodReference interceptMethod = module.ImportMethod<IInterceptor>("Intercept");
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, interceptMethod);
            IL.PackageReturnValue(module, returnType);
        }

        private static void GetMethodReplacementInstance(MethodDefinition method,
                                                         CilWorker IL,
                                                         VariableDefinition methodReplacement,
                                                         VariableDefinition methodReplacementProvider,
                                                         VariableDefinition invocationInfo)
        {
            TypeDefinition declaringType = method.DeclaringType;
            ModuleDefinition module = declaringType.Module;
            Instruction pushInstance = method.HasThis ? IL.Create(OpCodes.Ldarg_0) : IL.Create(OpCodes.Ldnull);

            MethodReference getReplacement = module.ImportMethod<IMethodReplacementProvider>("GetMethodReplacement");
            IL.Emit(OpCodes.Ldloc, methodReplacementProvider);

            Instruction skipGetMethodReplacement = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Brfalse, skipGetMethodReplacement);
            IL.Emit(OpCodes.Ldloc, methodReplacementProvider);

            IL.Append(pushInstance);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, getReplacement);
            IL.Emit(OpCodes.Stloc, methodReplacement);

            IL.Append(skipGetMethodReplacement);
        }
    }
}