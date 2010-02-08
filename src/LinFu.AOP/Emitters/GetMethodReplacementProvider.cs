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
    public class GetMethodReplacementProvider : IInstructionEmitter
    {
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly MethodDefinition _hostMethod;
        public GetMethodReplacementProvider(MethodDefinition hostMethod, VariableDefinition methodReplacementProvider)
        {
            _hostMethod = hostMethod;
            _methodReplacementProvider = methodReplacementProvider;
        }

        public void Emit(CilWorker IL)
        {
            var method = _hostMethod;
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
                return;
            }

            var getProvider = module.ImportMethod<IMethodReplacementHost>("get_MethodReplacementProvider");
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getProvider);
            IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
        }
    }
}
