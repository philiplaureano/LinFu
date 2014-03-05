using System.Collections.Generic;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a method body rewriter that surrounds a method body with the necessary prolog and epilogs
    /// that enable method body interception.
    /// </summary>
    public class InterceptAndSurroundMethodBody : IMethodBodyRewriter
    {
        private readonly IInstructionEmitter _addMethodReplacement;
        private readonly IEmitInvocationInfo _emitter;
        private readonly IInstructionEmitter _getClassMethodReplacementProvider;
        private readonly IInstructionEmitter _getInstanceMethodReplacementProvider;
        private readonly IInstructionEmitter _getInterceptionDisabled;
        private readonly VariableDefinition _interceptionDisabled;
        private readonly IMethodBodyRewriterParameters _parameters;
        private readonly ISurroundMethodBody _surroundMethodBody;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptAndSurroundMethodBody"/> class.
        /// </summary>
        /// <param name="emitter">The emitter that will instantiate the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="getInterceptionDisabled">The emitter that will determine whether or not method interception is enabled.</param>
        /// <param name="surroundMethodBody">The <see cref="ISurroundMethodBody"/> instance that will add the epilogs and prologs to the method body.</param>
        /// <param name="getInstanceMethodReplacementProvider">The emitter that will obtain the method replacement provider instance.</param>
        /// <param name="getClassMethodReplacementProvider">The emitter that will obtain the class-level method replacement provider instance.</param>
        /// <param name="addMethodReplacement">The instruction emitter that will add the call to obtain the method body replacement instance. </param>
        /// <param name="parameters">The parameters that describe the context of the method body rewrite.</param>
        public InterceptAndSurroundMethodBody(IEmitInvocationInfo emitter,
            IInstructionEmitter getInterceptionDisabled,
            ISurroundMethodBody surroundMethodBody,
            IInstructionEmitter getInstanceMethodReplacementProvider,
            IInstructionEmitter getClassMethodReplacementProvider,
            IInstructionEmitter addMethodReplacement,
            IMethodBodyRewriterParameters parameters)
        {
            _getInterceptionDisabled = getInterceptionDisabled;
            _surroundMethodBody = surroundMethodBody;
            _getInstanceMethodReplacementProvider = getInstanceMethodReplacementProvider;
            _getClassMethodReplacementProvider = getClassMethodReplacementProvider;
            _addMethodReplacement = addMethodReplacement;
            _parameters = parameters;
            _emitter = emitter;

            _interceptionDisabled = parameters.InterceptionDisabled;
        }


        /// <summary>
        /// Rewrites a target method using the given CilWorker.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The CilWorker that will be used to rewrite the target method.</param>
        /// <param name="oldInstructions">The original instructions from the target method body.</param>
        public void Rewrite(MethodDefinition method, CilWorker IL,
            IEnumerable<Instruction> oldInstructions)
        {
            var targetMethod = _parameters.TargetMethod;
            var worker = targetMethod.GetILGenerator();
            var module = worker.GetModule();


            _getInterceptionDisabled.Emit(worker);

            // Construct the InvocationInfo instance
            var skipInvocationInfo = worker.Create(OpCodes.Nop);
            worker.Emit(OpCodes.Ldloc, _parameters.InterceptionDisabled);
            worker.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var interceptedMethod = targetMethod;
            _emitter.Emit(targetMethod, interceptedMethod, _parameters.InvocationInfo);

            var skipGetReplacementProvider = IL.Create(OpCodes.Nop);

            // var provider = this.MethodReplacementProvider;
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipGetReplacementProvider);

            _getInstanceMethodReplacementProvider.Emit(IL);
            _surroundMethodBody.AddProlog(worker);

            IL.Append(skipGetReplacementProvider);

            worker.Append(skipInvocationInfo);
            _getClassMethodReplacementProvider.Emit(worker);


            var returnType = targetMethod.ReturnType.ReturnType;
            _addMethodReplacement.Emit(worker);

            // Save the return value
            var voidType = module.Import(typeof (void));
            _surroundMethodBody.AddEpilog(worker);

            //if (returnType != voidType)
            //    worker.Emit(OpCodes.Ldloc, _parameters.ReturnValue);

            worker.Emit(OpCodes.Ret);
        }
    }
}