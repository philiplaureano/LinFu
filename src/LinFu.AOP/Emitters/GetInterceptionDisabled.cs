using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a class that emits the instructions that determine whether or not method interception is disabled.
    /// </summary>
    public class GetInterceptionDisabled : IInstructionEmitter
    {
        private readonly MethodReference _hostMethod;
        private readonly VariableDefinition _interceptionDisabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetInterceptionDisabled"/> class.
        /// </summary>
        /// <param name="parameters">The <see cref="IMethodBodyRewriterParameters"/> instance.</param>
        public GetInterceptionDisabled(IMethodBodyRewriterParameters parameters)
        {
            _hostMethod = parameters.TargetMethod;
            _interceptionDisabled = parameters.InterceptionDisabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetInterceptionDisabled"/> class.
        /// </summary>
        /// <param name="hostMethod">The target method.</param>
        /// <param name="interceptionDisabled">The local variable that determines whether or not method interception is disabled.</param>
        public GetInterceptionDisabled(MethodReference hostMethod, VariableDefinition interceptionDisabled)
        {
            _hostMethod = hostMethod;
            _interceptionDisabled = interceptionDisabled;
        }

        #region IInstructionEmitter Members

        /// <summary>
        /// Emits the instructions that determine whether or not method interception is disabled.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> instance responsible for adding or removing instructions to the method body.</param>
        public void Emit(ILProcessor IL)
        {
            ModuleDefinition module = IL.GetModule();
            TypeReference modifiableType = module.ImportType<IModifiableType>();
            MethodReference getInterceptionDisabledMethod =
                module.ImportMethod<IModifiableType>("get_IsInterceptionDisabled");
            if (!_hostMethod.HasThis)
            {
                IL.Emit(OpCodes.Ldc_I4_0);
                IL.Emit(OpCodes.Stloc, _interceptionDisabled);
                return;
            }

            Instruction skipLabel = IL.Create(OpCodes.Nop);

            // var interceptionDisabled = this.IsInterceptionDisabled;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Brfalse, skipLabel);

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, getInterceptionDisabledMethod);
            IL.Emit(OpCodes.Stloc, _interceptionDisabled);

            IL.Append(skipLabel);
        }

        #endregion
    }
}