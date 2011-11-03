using System;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a class that emits the instructions that obtain a class-level <see cref="IMethodReplacementProvider"/> instance.
    /// </summary>
    public class GetClassMethodReplacementProvider : IInstructionEmitter
    {
        private readonly VariableDefinition _classMethodReplacementProvider;
        private readonly VariableDefinition _invocationInfo;
        private readonly Func<ModuleDefinition, MethodReference> _resolveGetProviderMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetClassMethodReplacementProvider"/> class.
        /// </summary>
        /// <param name="parameters">The method body rewriter paramters that describe the </param>
        /// <param name="resolveGetProviderMethod">The functor that resolves the method that obtains the method replacement provider instance.</param>
        public GetClassMethodReplacementProvider(IMethodBodyRewriterParameters parameters,
                                                 Func<ModuleDefinition, MethodReference> resolveGetProviderMethod)
        {
            _invocationInfo = parameters.InvocationInfo;
            _classMethodReplacementProvider = parameters.ClassMethodReplacementProvider;
            _resolveGetProviderMethod = resolveGetProviderMethod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetClassMethodReplacementProvider"/> class.
        /// </summary>
        /// <param name="invocationInfo">The variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="classMethodReplacementProvider">The variable that contains the class method replacement provider instance.</param>
        /// <param name="resolveGetProviderMethod">The functor that resolves the method that obtains the method replacement provider instance.</param>
        public GetClassMethodReplacementProvider(VariableDefinition invocationInfo,
                                                 VariableDefinition classMethodReplacementProvider,
                                                 Func<ModuleDefinition, MethodReference> resolveGetProviderMethod)
        {
            _invocationInfo = invocationInfo;
            _classMethodReplacementProvider = classMethodReplacementProvider;
            _resolveGetProviderMethod = resolveGetProviderMethod;
        }

        #region IInstructionEmitter Members

        /// <summary>
        /// Emits the instructions that obtain a class-level <see cref="IMethodReplacementProvider"/> instance.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> instance that points to the instructions in the method body.</param>
        public void Emit(ILProcessor IL)
        {
            ModuleDefinition module = IL.GetModule();
            MethodDefinition method = IL.GetMethod();

            MethodReference getProvider = _resolveGetProviderMethod(module);

            OpCode pushThis = method.HasThis ? OpCodes.Ldarg_0 : OpCodes.Ldnull;
            IL.Emit(pushThis);

            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, getProvider);
            IL.Emit(OpCodes.Stloc, _classMethodReplacementProvider);
        }

        #endregion
    }
}