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
    public class GetInterceptionDisabled : IInstructionEmitter
    {
        private MethodReference _hostMethod;
        private VariableDefinition _interceptionDisabled;

        public GetInterceptionDisabled(IMethodBodyRewriterParameters parameters)
        {
            _hostMethod = parameters.TargetMethod;
            _interceptionDisabled = parameters.InterceptionDisabled;
        }

        public GetInterceptionDisabled(MethodReference hostMethod, VariableDefinition interceptionDisabled)
        {
            _hostMethod = hostMethod;
            _interceptionDisabled = interceptionDisabled;
        }

        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();
            var modifiableType = module.ImportType<IModifiableType>();            
            var getInterceptionDisabledMethod = module.ImportMethod<IModifiableType>("get_IsInterceptionDisabled");
            if (!_hostMethod.HasThis)
            {
                IL.Emit(OpCodes.Ldc_I4_0);
                IL.Emit(OpCodes.Stloc, _interceptionDisabled);
                return;
            }

            // var interceptionDisabled = this.IsInterceptionDisabled;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, getInterceptionDisabledMethod);
            IL.Emit(OpCodes.Stloc, _interceptionDisabled);
        }
    }
}
