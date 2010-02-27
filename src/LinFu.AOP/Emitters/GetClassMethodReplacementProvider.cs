using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class GetClassMethodReplacementProvider : IInstructionEmitter
    {
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _classMethodReplacementProvider;
        private readonly Func<ModuleDefinition, MethodReference> _resolveGetProviderMethod;

        public GetClassMethodReplacementProvider(IMethodBodyRewriterParameters parameters, 
            Func<ModuleDefinition, MethodReference> resolveGetProviderMethod)
        {
            _invocationInfo = parameters.InvocationInfo;
            _classMethodReplacementProvider = parameters.ClassMethodReplacementProvider;
            _resolveGetProviderMethod = resolveGetProviderMethod;
        }
       
        public GetClassMethodReplacementProvider(VariableDefinition invocationInfo, VariableDefinition classMethodReplacementProvider, 
            Func<ModuleDefinition, MethodReference> resolveGetProviderMethod)
        {
            _invocationInfo = invocationInfo;
            _classMethodReplacementProvider = classMethodReplacementProvider;
            _resolveGetProviderMethod = resolveGetProviderMethod;
        }

        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();
            var method = IL.GetMethod();

            var getProvider = _resolveGetProviderMethod(module);

            var pushThis = method.HasThis ? OpCodes.Ldarg_0 : OpCodes.Ldnull;
            IL.Emit(pushThis);

            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, getProvider);
            IL.Emit(OpCodes.Stloc, _classMethodReplacementProvider);
        }        
    }
}
