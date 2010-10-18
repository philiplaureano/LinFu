using System;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a type that emits the call to the <see cref="IBeforeInvoke"/> instance.
    /// </summary>
    public class EmitBeforeInvoke : IInstructionEmitter
    {
        private readonly VariableDefinition _invocationInfo;
        private readonly Type _registryType;
        private readonly VariableDefinition _surroundingClassImplementation;
        private readonly VariableDefinition _surroundingImplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitBeforeInvoke"/> class.
        /// </summary>
        /// <param name="invocationInfo">The variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="surroundingClassImplementation">The variable that contains the class-level <see cref="IAroundInvoke"/> instance.</param>
        /// <param name="surroundingImplementation">The variable that contains the instance-level <see cref="IAroundInvoke"/> instance.</param>
        /// <param name="registryType">The interception registry type that will be responsible for handling class-level interception events.</param>
        public EmitBeforeInvoke(VariableDefinition invocationInfo,
                                VariableDefinition surroundingClassImplementation,
                                VariableDefinition surroundingImplementation,
                                Type registryType)
        {
            _invocationInfo = invocationInfo;
            _surroundingClassImplementation = surroundingClassImplementation;
            _surroundingImplementation = surroundingImplementation;
            _registryType = registryType;
        }

        #region IInstructionEmitter Members

        /// <summary>
        /// Emits the call to the <see cref="IAfterInvoke"/> instance.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that points to the current method body.</param>
        public void Emit(CilWorker IL)
        {
            MethodDefinition targetMethod = IL.GetMethod();
            TypeDefinition declaringType = targetMethod.DeclaringType;
            ModuleDefinition module = declaringType.Module;

            var getSurroundingClassImplementation = new GetSurroundingClassImplementation(_invocationInfo,
                                                                                          _surroundingClassImplementation,
                                                                                          _registryType.GetMethod(
                                                                                              "GetSurroundingImplementation"));

            // var classAroundInvoke = AroundInvokeRegistry.GetSurroundingImplementation(info);           
            getSurroundingClassImplementation.Emit(IL);

            // classAroundInvoke.BeforeInvoke(info);
            Instruction skipInvoke = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldloc, _surroundingClassImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            MethodReference beforeInvoke = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, _surroundingClassImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke);

            IL.Append(skipInvoke);

            // if (surroundingImplementation != null) {
            if (!targetMethod.HasThis)
                return;

            Instruction skipInvoke1 = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke1);

            MethodReference beforeInvoke1 = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke1);

            IL.Append(skipInvoke1);
            // }
        }

        #endregion
    }
}