using System;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a class that surrounds a call site with calls to an <see cref="IAroundInvoke"/> instance.
    /// </summary>
    public class SurroundMethodBody : ISurroundMethodBody
    {
        private readonly VariableDefinition _aroundInvokeProvider;
        private readonly IInstructionEmitter _getMethodReplacementProvider;
        private readonly VariableDefinition _interceptionDisabled;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly string _providerName;
        private readonly Type _registryType;
        private readonly VariableDefinition _returnValue;

        private VariableDefinition _surroundingClassImplementation;
        private VariableDefinition _surroundingImplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="IMethodBodyRewriterParameters"/> class.
        /// </summary>
        /// <param name="parameters">The parameters that describe the context of the emitter call.</param>
        /// <param name="providerName">The name of the <see cref="IAroundInvokeProvider"/> property.</param>
        public SurroundMethodBody(IMethodBodyRewriterParameters parameters, string providerName)
        {
            _methodReplacementProvider = parameters.MethodReplacementProvider;
            _aroundInvokeProvider = parameters.AroundInvokeProvider;
            _invocationInfo = parameters.InvocationInfo;
            _returnValue = parameters.ReturnValue;
            _interceptionDisabled = parameters.InterceptionDisabled;
            _providerName = providerName;

            var getMethodReplacementProvider = new GetMethodReplacementProvider(_methodReplacementProvider,
                                                                                parameters.TargetMethod,
                                                                                parameters.
                                                                                    GetMethodReplacementProviderMethod);

            _getMethodReplacementProvider = getMethodReplacementProvider;
            _registryType = parameters.RegistryType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IMethodBodyRewriterParameters"/> class.
        /// </summary>
        /// <param name="methodReplacementProvider">The variable that contains the <see cref="IMethodReplacementProvider"/> instance.</param>
        /// <param name="aroundInvokeProvider">The variable that contains the <see cref="IAroundInvokeProvider"/> instance</param>
        /// <param name="invocationInfo">The variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="interceptionDisabled">The variable that determines whether or not interception is disabled</param>
        /// <param name="returnValue">The variable that contains the method return value.</param>
        /// <param name="registryType">The interception registry type that will be responsible for handling class-level interception events.</param>
        /// <param name="providerName">The name of the <see cref="IAroundInvokeProvider"/> property.</param>
        public SurroundMethodBody(VariableDefinition methodReplacementProvider,
                                  VariableDefinition aroundInvokeProvider,
                                  VariableDefinition invocationInfo,
                                  VariableDefinition interceptionDisabled,
                                  VariableDefinition returnValue,
                                  Type registryType,
                                  string providerName)
        {
            _methodReplacementProvider = methodReplacementProvider;
            _aroundInvokeProvider = aroundInvokeProvider;
            _invocationInfo = invocationInfo;
            _interceptionDisabled = interceptionDisabled;
            _returnValue = returnValue;
            _registryType = registryType;
            _providerName = providerName;
        }

        #region ISurroundMethodBody Members

        /// <summary>
        /// Adds a prolog to the given method body.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that points to the given method body.</param>
        public void AddProlog(CilWorker IL)
        {
            MethodDefinition method = IL.GetMethod();
            _surroundingImplementation = method.AddLocal<IAroundInvoke>();
            _surroundingClassImplementation = method.AddLocal<IAroundInvoke>();

            Instruction skipProlog = IL.Create(OpCodes.Nop);
            TypeDefinition declaringType = method.DeclaringType;
            ModuleDefinition module = declaringType.Module;
            TypeReference modifiableType = module.ImportType<IModifiableType>();

            if (method.HasThis)
            {
                IL.Emit(OpCodes.Ldarg_0);
                IL.Emit(OpCodes.Isinst, modifiableType);
                IL.Emit(OpCodes.Brfalse, skipProlog);
            }

            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipProlog);

            // var provider = this.MethodReplacementProvider;

            if (_getMethodReplacementProvider != null)
                _getMethodReplacementProvider.Emit(IL);

            var getAroundInvokeProvider = new GetAroundInvokeProvider(_aroundInvokeProvider, _providerName);
            getAroundInvokeProvider.Emit(IL);

            // if (aroundInvokeProvider != null ) {
            Instruction skipGetSurroundingImplementation = IL.Create(OpCodes.Nop);
            var getSurroundingImplementationInstance = new GetSurroundingImplementationInstance(_aroundInvokeProvider,
                                                                                                _invocationInfo,
                                                                                                _surroundingImplementation,
                                                                                                skipGetSurroundingImplementation);

            getSurroundingImplementationInstance.Emit(IL);

            // }

            IL.Append(skipGetSurroundingImplementation);
            var emitBeforeInvoke = new EmitBeforeInvoke(_invocationInfo, _surroundingClassImplementation,
                                                        _surroundingImplementation, _registryType);
            emitBeforeInvoke.Emit(IL);

            IL.Append(skipProlog);
        }

        /// <summary>
        /// Adds an epilog to the given method body.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that points to the given method body.</param>
        public void AddEpilog(CilWorker IL)
        {
            Instruction skipEpilog = IL.Create(OpCodes.Nop);

            // if (!IsInterceptionDisabled && surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipEpilog);

            // surroundingImplementation.AfterInvoke(invocationInfo, returnValue);
            var emitAfterInvoke = new EmitAfterInvoke(_surroundingImplementation, _surroundingClassImplementation,
                                                      _invocationInfo, _returnValue);
            emitAfterInvoke.Emit(IL);

            // }
            IL.Append(skipEpilog);
        }

        #endregion
    }
}