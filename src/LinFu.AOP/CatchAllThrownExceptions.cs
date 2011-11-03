using System;
using System.Collections.Generic;
using System.Reflection;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a method rewriter that modifies a method body to support dynamic exception handling.
    /// </summary>
    public class CatchAllThrownExceptions : BaseMethodRewriter
    {
        private VariableDefinition _exception;
        private VariableDefinition _exceptionHandler;
        private VariableDefinition _exceptionInfo;
        private VariableDefinition _invocationInfo;
        private VariableDefinition _returnValue;
        private TypeReference _voidType;

        /// <summary>
        /// Adds additional references to the target module.
        /// </summary>
        /// <param name="module">The host module.</param>
        public override void ImportReferences(ModuleDefinition module)
        {
            _voidType = module.Import(typeof (void));
        }

        /// <summary>
        /// Adds local variables to the <paramref name="hostMethod"/>.
        /// </summary>
        /// <param name="hostMethod">The target method.</param>
        public override void AddLocals(MethodDefinition hostMethod)
        {
            _exception = hostMethod.AddLocal<Exception>();
            _invocationInfo = hostMethod.AddLocal<IInvocationInfo>();
            _exceptionHandler = hostMethod.AddLocal<IExceptionHandler>();
            _exceptionInfo = hostMethod.AddLocal<IExceptionHandlerInfo>();

            TypeReference returnType = hostMethod.ReturnType;
            if (returnType != _voidType)
                _returnValue = hostMethod.AddLocal<object>();
        }

        /// <summary>
        /// Rewrites the instructions in the target method body to support dynamic exception handling.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="IL">The <see cref="ILProcessor"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected override void RewriteMethodBody(MethodDefinition targetMethod, ILProcessor IL,
                                                  IEnumerable<Instruction> oldInstructions)
        {
            Instruction endOfOriginalInstructionBlock = IL.Create(OpCodes.Nop);
            var addOriginalInstructions = new AddOriginalInstructions(oldInstructions, endOfOriginalInstructionBlock);


            Instruction endLabel = IL.Create(OpCodes.Nop);
            Instruction tryStart = IL.Create(OpCodes.Nop);
            Instruction tryEnd = IL.Create(OpCodes.Nop);
            Instruction catchStart = IL.Create(OpCodes.Nop);
            Instruction catchEnd = IL.Create(OpCodes.Nop);

            ModuleDefinition module = IL.GetModule();
            var handler = new ExceptionHandler(ExceptionHandlerType.Catch);
            MethodBody body = targetMethod.Body;
            body.ExceptionHandlers.Add(handler);

            handler.CatchType = module.ImportType<Exception>();

            handler.TryStart = tryStart;
            handler.TryEnd = tryEnd;

            handler.HandlerStart = catchStart;
            handler.HandlerEnd = catchEnd;

            var emitter = new InvocationInfoEmitter(true);

            TypeReference returnType = targetMethod.ReturnType;

            // try {
            IL.Append(tryStart);
            addOriginalInstructions.Emit(IL);

            IL.Append(endOfOriginalInstructionBlock);
            if (returnType != _voidType && _returnValue != null)
            {
                IL.Emit(OpCodes.Stloc, _returnValue);
            }

            IL.Emit(OpCodes.Leave, endLabel);

            // }            
            IL.Append(tryEnd);
            // catch (Exception ex) {
            IL.Append(catchStart);
            IL.Emit(OpCodes.Stloc, _exception);

            SaveExceptionInfo(targetMethod, emitter);
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);

            MethodInfo getHandlerMethodInfo = typeof (ExceptionHandlerRegistry).GetMethod("GetHandler");
            MethodReference getHandlerMethod = module.Import(getHandlerMethodInfo);
            IL.Emit(OpCodes.Call, getHandlerMethod);
            IL.Emit(OpCodes.Stloc, _exceptionHandler);

            // if (exceptionHandler == null) 
            //      throw;
            Instruction doRethrow = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _exceptionHandler);
            IL.Emit(OpCodes.Brfalse, doRethrow);


            // if (handler.CanCatch(exceptionInfo)) {
            Instruction leaveBlock = IL.Create(OpCodes.Nop);
            MethodReference canCatch = module.ImportMethod<IExceptionHandler>("CanCatch");
            IL.Emit(OpCodes.Ldloc, _exceptionHandler);
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);
            IL.Emit(OpCodes.Callvirt, canCatch);
            IL.Emit(OpCodes.Brfalse, doRethrow);

            MethodReference catchMethod = module.ImportMethod<IExceptionHandler>("Catch");
            IL.Emit(OpCodes.Ldloc, _exceptionHandler);
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);
            IL.Emit(OpCodes.Callvirt, catchMethod);
            // }


            MethodReference getShouldSkipRethrow = module.ImportMethod<IExceptionHandlerInfo>("get_ShouldSkipRethrow");
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
                Instruction returnOriginalValue = IL.Create(OpCodes.Nop);
                MethodReference getReturnValue = module.ImportMethod<IExceptionHandlerInfo>("get_ReturnValue");

                IL.Emit(OpCodes.Ldloc, _exceptionInfo);
                IL.Emit(OpCodes.Brfalse, returnOriginalValue);

                IL.Emit(OpCodes.Ldloc, _exceptionInfo);
                IL.Emit(OpCodes.Callvirt, getReturnValue);
                IL.Emit(OpCodes.Stloc, _returnValue);
                IL.Append(returnOriginalValue);

                IL.Emit(OpCodes.Ldloc, _returnValue);
            }

            IL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Saves the current <see cref="IExceptionHandlerInfo"/> instance.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="emitter">The <see cref="IEmitInvocationInfo"/> instance that will emit the current method context.</param>
        private void SaveExceptionInfo(MethodDefinition targetMethod, IEmitInvocationInfo emitter)
        {
            ILProcessor IL = targetMethod.GetILGenerator();
            ModuleDefinition module = IL.GetModule();

            emitter.Emit(targetMethod, targetMethod, _invocationInfo);
            IL.Emit(OpCodes.Ldloc, _exception);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);

            MethodReference exceptionInfoConstructor = module.ImportConstructor<ExceptionHandlerInfo>(
                typeof (Exception),
                typeof (IInvocationInfo));
            IL.Emit(OpCodes.Newobj, exceptionInfoConstructor);
            IL.Emit(OpCodes.Stloc, _exceptionInfo);

            TypeReference returnType = targetMethod.ReturnType;
            if (returnType == _voidType || _returnValue == null)
                return;

            // exceptionInfo.ReturnValue = returnValue;
            MethodReference setReturnValue = module.ImportMethod<IExceptionHandlerInfo>("set_ReturnValue");
            IL.Emit(OpCodes.Ldloc, _exceptionInfo);
            IL.Emit(OpCodes.Ldloc, _returnValue);
            IL.Emit(OpCodes.Callvirt, setReturnValue);
        }
    }
}