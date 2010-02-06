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
        private IEmitInvocationInfo _emitter;
        private ModuleDefinition _module;

        public InterceptAndSurroundMethodBody(ModuleDefinition module)
            : this(module, new InvocationInfoEmitter())
        {
        }

        public InterceptAndSurroundMethodBody(ModuleDefinition module, IEmitInvocationInfo emitter)
        {
            _module = module;
            _emitter = emitter;
        }

        public void Rewrite(MethodDefinition method, CilWorker IL,
            IEnumerable<Instruction> oldInstructions)
        {
            var interceptionDisabled = method.AddLocal<bool>();
            var invocationInfo = method.AddLocal<IInvocationInfo>();
            var aroundInvokeProvider = method.AddLocal<IAroundInvokeProvider>();
            var methodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();

            var returnValue = method.AddLocal<object>();
            var classMethodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();

            var getInterceptionDisabled = new GetInterceptionDisabled(method, interceptionDisabled);
            getInterceptionDisabled.Emit(IL);

            // Construct the InvocationInfo instance
            var skipInvocationInfo = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var targetMethod = method;
            var interceptedMethod = method;
            _emitter.Emit(targetMethod, interceptedMethod, invocationInfo);


            var surroundMethodBody = new SurroundMethodBody(methodReplacementProvider, aroundInvokeProvider,
                                                            invocationInfo, interceptionDisabled, returnValue);
            surroundMethodBody.AddProlog(method, IL);

            IL.Append(skipInvocationInfo);


            var getClassMethodReplacementProvider = new GetClassMethodReplacementProvider(invocationInfo, classMethodReplacementProvider);
            getClassMethodReplacementProvider.Emit(IL);

            var returnType = method.ReturnType.ReturnType;
            var addMethodReplacement = new AddMethodReplacementImplementation(oldInstructions, interceptionDisabled,
                                                                              methodReplacementProvider,
                                                                              classMethodReplacementProvider,
                                                                              invocationInfo, returnValue);
            addMethodReplacement.Emit(IL);

            // Save the return value
            TypeReference voidType = _module.Import(typeof(void));
            surroundMethodBody.AddEpilog(method, IL);

            if (returnType != voidType)
                IL.Emit(OpCodes.Ldloc, returnValue);

            IL.Emit(OpCodes.Ret);
        }
    }
}
