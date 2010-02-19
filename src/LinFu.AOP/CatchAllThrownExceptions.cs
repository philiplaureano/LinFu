using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class CatchAllThrownExceptions : BaseMethodRewriter
    {
        private VariableDefinition _exception;
        private VariableDefinition _invocationInfo;
        private VariableDefinition _exceptionHandler;
        private VariableDefinition _exceptionInfo;
        private VariableDefinition _returnValue;
        private TypeReference _voidType;

        public override void ImportReferences(ModuleDefinition module)
        {
            _voidType = module.Import(typeof(void));
        }

        public override void AddLocals(MethodDefinition hostMethod)
        {
            _exception = hostMethod.AddLocal<Exception>();
            _invocationInfo = hostMethod.AddLocal<IInvocationInfo>();
            _exceptionHandler = hostMethod.AddLocal<IExceptionHandler>();
            _exceptionInfo = hostMethod.AddLocal<IExceptionHandlerInfo>();

            var returnType = hostMethod.ReturnType.ReturnType;
            if (returnType != _voidType)
                _returnValue = hostMethod.AddLocal<object>();

        }

        protected override void RewriteMethodBody(MethodDefinition targetMethod, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var endOfOriginalInstructionBlock = IL.Create(OpCodes.Nop);
            var addOriginalInstructions = new AddOriginalInstructions(oldInstructions, endOfOriginalInstructionBlock);


            var endLabel = IL.Create(OpCodes.Nop);
            var tryStart = IL.Emit(OpCodes.Nop);
            var tryEnd = IL.Emit(OpCodes.Nop);
            var catchStart = IL.Emit(OpCodes.Nop);
            var catchEnd = IL.Emit(OpCodes.Nop);

            var module = IL.GetModule();
            var handler = new ExceptionHandler(ExceptionHandlerType.Catch);
            var body = targetMethod.Body;
            body.ExceptionHandlers.Add(handler);

            handler.CatchType = module.ImportType<Exception>();

            handler.TryStart = tryStart;
            handler.TryEnd = tryEnd;

            handler.HandlerStart = catchStart;
            handler.HandlerEnd = catchEnd;

            var emitter = new InvocationInfoEmitter(true);

            var returnType = targetMethod.ReturnType.ReturnType;

            // try {
            IL.Append(tryStart);
            addOriginalInstructions.Emit(IL);

            if (returnType != _voidType && _returnValue != null)
            {
                // exceptionInfo.ReturnValue = returnValue;
                var setReturnValue = module.ImportMethod<IExceptionHandlerInfo>("set_ReturnValue");
                IL.Emit(OpCodes.Ldloc, _exceptionInfo);
                IL.Emit(OpCodes.Ldloc, _returnValue);
                IL.Emit(OpCodes.Callvirt, setReturnValue);
            }

            IL.Append(endOfOriginalInstructionBlock);

            IL.Emit(OpCodes.Leave, endLabel);
            // }
            IL.Append(tryEnd);
            // catch (Exception ex) {
            IL.Append(catchStart);
            IL.Emit(OpCodes.Stloc, _exception);

            emitter.Emit(targetMethod, targetMethod, _invocationInfo);
            IL.Emit(OpCodes.Ldloc, _exception);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);

            var exceptionInfoConstructor = module.ImportConstructor<ExceptionHandlerInfo>(typeof(Exception),
                                                                                          typeof(IInvocationInfo));

            IL.Emit(OpCodes.Newobj, exceptionInfoConstructor);
            IL.Emit(OpCodes.Stloc, _exceptionInfo);
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);

            var getHandlerMethodInfo = typeof(ExceptionHandlerRegistry).GetMethod("GetHandler");
            var getHandlerMethod = module.Import(getHandlerMethodInfo);
            IL.Emit(OpCodes.Call, getHandlerMethod);
            IL.Emit(OpCodes.Stloc, _exceptionHandler);

            // if (exceptionHandler == null) 
            //      throw;
            var doRethrow = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _exceptionHandler);
            IL.Emit(OpCodes.Brfalse, doRethrow);

            // if (handler.CanCatch(exceptionInfo)) {
            var leaveBlock = IL.Create(OpCodes.Nop);
            var canCatch = module.ImportMethod<IExceptionHandler>("CanCatch");
            IL.Emit(OpCodes.Ldloc, _exceptionHandler);
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);
            IL.Emit(OpCodes.Callvirt, canCatch);
            IL.Emit(OpCodes.Brfalse, doRethrow);

            var catchMethod = module.ImportMethod<IExceptionHandler>("Catch");
            IL.Emit(OpCodes.Ldloc, _exceptionHandler);
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);
            IL.Emit(OpCodes.Callvirt, catchMethod);
            // }

            var getShouldSkipRethrow = module.ImportMethod<IExceptionHandlerInfo>("get_ShouldSkipRethrow");
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);
            IL.Emit(OpCodes.Callvirt, getShouldSkipRethrow);
            IL.Emit(OpCodes.Brfalse, doRethrow);

            IL.Emit(OpCodes.Br, leaveBlock);

            IL.Append(doRethrow);
            IL.Emit(OpCodes.Rethrow);

            IL.Append(leaveBlock);
            IL.Emit(OpCodes.Leave, endLabel);
            IL.Append(catchEnd);
            // }
            IL.Append(endLabel);

            if (returnType != _voidType && _returnValue != null)
            {
                var getReturnValue = module.ImportMethod<IExceptionHandlerInfo>("get_ReturnValue");

                IL.Emit(OpCodes.Ldloc, _exceptionInfo);
                IL.Emit(OpCodes.Callvirt, getReturnValue);
            }

            IL.Emit(OpCodes.Ret);
        }
    }
}
