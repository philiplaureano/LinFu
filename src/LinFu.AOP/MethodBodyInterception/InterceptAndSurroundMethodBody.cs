using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;
using LinFu.Reflection.Emit;

namespace LinFu.AOP.Cecil
{    
    public class InterceptAndSurroundMethodBody : IMethodBodyRewriter
    {
        private readonly IEmitInvocationInfo _emitter;
        private readonly IInstructionEmitter _getInterceptionDisabled;
        private readonly ISurroundMethodBody _surroundMethodBody;
        private readonly IInstructionEmitter _getClassMethodReplacementProvider;
        private readonly IInstructionEmitter _getInstanceMethodReplacementProvider;
        private readonly IInstructionEmitter _addMethodReplacement;
        private readonly IMethodBodyRewriterParameters _parameters;
        private readonly VariableDefinition _interceptionDisabled;

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
            //IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            //IL.Emit(OpCodes.Brtrue, skipGetReplacementProvider);
            _getInstanceMethodReplacementProvider.Emit(IL);
            _surroundMethodBody.AddProlog(worker);
            IL.Append(skipGetReplacementProvider);

            worker.Append(skipInvocationInfo);
            
            _getClassMethodReplacementProvider.Emit(worker);

            

            var returnType = targetMethod.ReturnType.ReturnType;
            
            _addMethodReplacement.Emit(worker);

            // Save the return value
            TypeReference voidType = module.Import(typeof(void));
            _surroundMethodBody.AddEpilog(worker);

            if (returnType != voidType)
                worker.Emit(OpCodes.Ldloc, _parameters.ReturnValue);

            worker.Emit(OpCodes.Ret);
        }
    }
}
