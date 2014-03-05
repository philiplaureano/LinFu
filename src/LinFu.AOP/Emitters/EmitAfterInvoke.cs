using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a type that emits the call to the <see cref="IAfterInvoke"/> instance.
    /// </summary>
    public class EmitAfterInvoke : IInstructionEmitter
    {
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _returnValue;
        private readonly VariableDefinition _surroundingClassImplementation;
        private readonly VariableDefinition _surroundingImplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitAfterInvoke"/> class.
        /// </summary>
        /// <param name="surroundingImplementation">The variable that contains the <see cref="IAroundInvoke"/> instance.</param>
        /// <param name="surroundingClassImplementation">The variable that contains the class-level <see cref="IAroundInvoke"/> instance.</param>
        /// <param name="invocationInfo">The variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="returnValue">The local vaiable that contains the return value of the target method.</param>
        public EmitAfterInvoke(VariableDefinition surroundingImplementation,
                               VariableDefinition surroundingClassImplementation,
                               VariableDefinition invocationInfo, VariableDefinition returnValue)
        {
            _surroundingImplementation = surroundingImplementation;
            _surroundingClassImplementation = surroundingClassImplementation;
            _invocationInfo = invocationInfo;
            _returnValue = returnValue;
        }

        #region IInstructionEmitter Members

        /// <summary>
        /// Emits the call to the <see cref="IAfterInvoke"/> instance.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that points to the current method body.</param>
        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();

            // instanceAroundInvoke.AfterInvoke(info, returnValue);
            Emit(IL, module, _surroundingImplementation, _invocationInfo, _returnValue);

            // classAroundInvoke.AfterInvoke(info, returnValue);
            Emit(IL, module, _surroundingClassImplementation, _invocationInfo, _returnValue);
        }

        #endregion

        private static void Emit(CilWorker IL, ModuleDefinition module,
                                 VariableDefinition surroundingImplementation,
                                 VariableDefinition invocationInfo,
                                 VariableDefinition returnValue)
        {
            var skipInvoke = IL.Create(OpCodes.Nop);

            var skipPrint = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brtrue, skipPrint);

            IL.Append(skipPrint);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            var aroundInvoke = module.ImportMethod<IAfterInvoke>("AfterInvoke");
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Ldloc, returnValue);
            IL.Emit(OpCodes.Callvirt, aroundInvoke);

            IL.Append(skipInvoke);
        }
    }
}