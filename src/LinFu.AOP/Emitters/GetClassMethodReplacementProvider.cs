using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class GetClassMethodReplacementProvider : IInstructionEmitter
    {
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _classMethodReplacementProvider;

        public GetClassMethodReplacementProvider(IMethodBodyRewriterParameters parameters)
        {
            _invocationInfo = parameters.InvocationInfo;
            _classMethodReplacementProvider = parameters.ClassMethodReplacementProvider;
        }

        public GetClassMethodReplacementProvider(VariableDefinition invocationInfo, VariableDefinition classMethodReplacementProvider)
        {
            _invocationInfo = invocationInfo;
            _classMethodReplacementProvider = classMethodReplacementProvider;
        }

        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();
            var method = IL.GetMethod();

            var getProvider = module.Import(typeof(MethodReplacementProviderRegistry).GetMethod("GetProvider"));

            if (method.HasThis)
                IL.Emit(OpCodes.Ldarg_0);
            else
                IL.Emit(OpCodes.Ldnull);

            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, getProvider);
            IL.Emit(OpCodes.Stloc, _classMethodReplacementProvider);
        }
    }
}
