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
    /// <summary>
    /// Represents a class that emits the instructions that obtain the current <see cref="IAroundInvoke"/> instance.
    /// </summary>
    public class GetSurroundingImplementationInstance : IInstructionEmitter
    {
        private readonly VariableDefinition _aroundInvokeProvider;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _surroundingImplementation;
        private readonly Instruction _skipGetSurroundingImplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSurroundingImplementationInstance"/> class.
        /// </summary>
        /// <param name="aroundInvokeProvider">The variable that will hold the <see cref="IAroundInvokeProvider"/> instance.</param>
        /// <param name="invocationInfo"></param>
        /// <param name="surroundingImplementation"></param>
        /// <param name="skipGetSurroundingImplementation"></param>
        public GetSurroundingImplementationInstance(VariableDefinition aroundInvokeProvider, VariableDefinition invocationInfo, VariableDefinition surroundingImplementation, Instruction skipGetSurroundingImplementation)
        {
            _aroundInvokeProvider = aroundInvokeProvider;
            _invocationInfo = invocationInfo;
            _surroundingImplementation = surroundingImplementation;
            _skipGetSurroundingImplementation = skipGetSurroundingImplementation;
        }

        /// <summary>
        /// Emits the instructions that obtain the current <see cref="IAroundInvoke"/> instance.
        /// </summary>
        /// <param name="IL"></param>
        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();

            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Brfalse, _skipGetSurroundingImplementation);

            // var surroundingImplementation = this.GetSurroundingImplementation(this, invocationInfo);
            var getSurroundingImplementation = module.ImportMethod<IAroundInvokeProvider>("GetSurroundingImplementation");
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingImplementation);
        }
    }
}
