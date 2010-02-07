using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public interface IMethodBodyRewriterParameters
    {
        MethodDefinition TargetMethod { get; }
        VariableDefinition AroundInvokeProvider { get; }
        VariableDefinition MethodReplacementProvider { get; }
        VariableDefinition ClassMethodReplacementProvider { get; }
        VariableDefinition InterceptionDisabled { get; }
        VariableDefinition InvocationInfo { get; }
        VariableDefinition ReturnValue { get; }
        IEnumerable<Instruction> OldInstructions { get; }
    }
}