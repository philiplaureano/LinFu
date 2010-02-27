using System;
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

        Type RegistryType { get; }
        Func<ModuleDefinition, MethodReference> GetMethodReplacementProviderMethod { get; }
        IEnumerable<Instruction> OldInstructions { get; }
    }
}