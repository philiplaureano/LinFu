using System.Reflection;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a class that emits the instructions that obtain the <see cref="IAroundInvoke"/> instance.
    /// </summary>
    public class GetSurroundingClassImplementation : IInstructionEmitter
    {
        private readonly MethodInfo _getSurroundingImplementationMethod;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _surroundingClassImplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSurroundingClassImplementation"/> class.
        /// </summary>
        /// <param name="invocationInfo">The variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="surroundingClassImplementation">The variable that contains the <see cref="IAroundInvoke"/> instance.</param>
        /// <param name="getSurroundingImplementationMethod">The method that will obtain the <see cref="IAroundInvoke"/> instance.</param>
        public GetSurroundingClassImplementation(VariableDefinition invocationInfo,
                                                 VariableDefinition surroundingClassImplementation,
                                                 MethodInfo getSurroundingImplementationMethod)
        {
            _invocationInfo = invocationInfo;
            _surroundingClassImplementation = surroundingClassImplementation;
            _getSurroundingImplementationMethod = getSurroundingImplementationMethod;
        }

        #region IInstructionEmitter Members

        /// <summary>
        /// Emits the instructions that obtain the <see cref="IAroundInvoke"/> instance.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> that points to the current method body.</param>
        public void Emit(ILProcessor IL)
        {
            MethodDefinition method = IL.GetMethod();
            TypeDefinition declaringType = method.DeclaringType;
            ModuleDefinition module = declaringType.Module;

            MethodReference getSurroundingImplementation = module.Import(_getSurroundingImplementationMethod);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingClassImplementation);
        }

        #endregion
    }
}