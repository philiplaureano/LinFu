using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class MethodBodyRewriterParameters : IMethodBodyRewriterParameters
    {
        private readonly CilWorker _cilWorker;
        private readonly VariableDefinition _interceptionDisabled;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _returnValue;
        private readonly VariableDefinition _aroundInvokeProvider;
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly VariableDefinition _classMethodReplacementProvider;
        private readonly IEnumerable<Instruction> _oldInstructions;

        public MethodBodyRewriterParameters(CilWorker IL, IEnumerable<Instruction> oldInstructions, VariableDefinition interceptionDisabled, VariableDefinition invocationInfo, VariableDefinition returnValue,
            VariableDefinition methodReplacementProvider, VariableDefinition aroundInvokeProvider, VariableDefinition classMethodReplacementProvider)
        {
            _cilWorker = IL;
            _oldInstructions = oldInstructions;
            _interceptionDisabled = interceptionDisabled;
            _invocationInfo = invocationInfo;
            _returnValue = returnValue;
            _methodReplacementProvider = methodReplacementProvider;
            _aroundInvokeProvider = aroundInvokeProvider;
            _classMethodReplacementProvider = classMethodReplacementProvider;
        }

        public IEnumerable<Instruction> OldInstructions
        {
            get { return _oldInstructions; }
        }

        public VariableDefinition ClassMethodReplacementProvider
        {
            get { return _classMethodReplacementProvider; }
        }

        public VariableDefinition AroundInvokeProvider
        {
            get { return _aroundInvokeProvider; }
        }

        public VariableDefinition MethodReplacementProvider
        {
            get { return _methodReplacementProvider; }
        }

        public MethodDefinition TargetMethod
        {
            get { return _cilWorker.GetMethod(); }
        }

        public VariableDefinition InterceptionDisabled
        {
            get { return _interceptionDisabled; }
        }

        public VariableDefinition InvocationInfo
        {
            get { return _invocationInfo; }
        }

        public VariableDefinition ReturnValue
        {
            get { return _returnValue; }
        }
    }
}
