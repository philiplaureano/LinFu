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
        private readonly IInstructionEmitter _addMethodReplacement;
        private readonly IMethodBodyRewriterParameters _parameters;

        public InterceptAndSurroundMethodBody(IEmitInvocationInfo emitter, 
            IInstructionEmitter getInterceptionDisabled, 
            ISurroundMethodBody surroundMethodBody, 
            IInstructionEmitter getClassMethodReplacementProvider, 
            IInstructionEmitter addMethodReplacement, 
            IMethodBodyRewriterParameters parameters)
        {
            _getInterceptionDisabled = getInterceptionDisabled;
            _surroundMethodBody = surroundMethodBody;
            _getClassMethodReplacementProvider = getClassMethodReplacementProvider;
            _addMethodReplacement = addMethodReplacement;
            _parameters = parameters;
            _emitter = emitter;
        }

        public void Rewrite(MethodDefinition method, CilWorker IL,
            IEnumerable<Instruction> oldInstructions)
        {
            var method1 = _parameters.TargetMethod;
            var worker = method1.GetILGenerator();
            var module = worker.GetModule();
            _getInterceptionDisabled.Emit(worker);

            // Construct the InvocationInfo instance
            var skipInvocationInfo = worker.Create(OpCodes.Nop);
            worker.Emit(OpCodes.Ldloc, _parameters.InterceptionDisabled);
            worker.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var targetMethod = method1;
            var interceptedMethod = method1;
            _emitter.Emit(targetMethod, interceptedMethod, _parameters.InvocationInfo);

            
            _surroundMethodBody.AddProlog(worker);
            worker.Append(skipInvocationInfo);
            
            _getClassMethodReplacementProvider.Emit(worker);

            var returnType = method1.ReturnType.ReturnType;
            
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
