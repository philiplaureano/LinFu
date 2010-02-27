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
        private readonly Func<ModuleDefinition, MethodReference> _resolveGetProviderMethod;        

        public GetMethodReplacementProvider(VariableDefinition methodReplacementProvider, MethodDefinition hostMethod, Func<ModuleDefinition, MethodReference> resolveGetProviderMethod)
        {
            _methodReplacementProvider = methodReplacementProvider;
            _hostMethod = hostMethod;
            _resolveGetProviderMethod = resolveGetProviderMethod;
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

            var getProvider = _resolveGetProviderMethod(module);
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getProvider);
            IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
        }
    }
}
