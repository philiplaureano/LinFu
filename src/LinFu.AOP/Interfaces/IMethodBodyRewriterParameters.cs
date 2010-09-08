using System;
using System.Collections.Generic;
using LinFu.AOP.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents the parameters used to add interception to a given method body.
    /// </summary>
    public interface IMethodBodyRewriterParameters
    {
        /// <summary>
        /// Gets the value indicating the TargetMethod to be modified.
        /// </summary>
        /// <value>The method to be modified.</value>
        MethodDefinition TargetMethod { get; }

        /// <summary>
        /// Gets the value indicating the local variable used to store the <see cref="IAroundInvokeProvider"/> instance.
        /// </summary>
        /// <value>The <see cref="IAroundInvokeProvider"/> instance.</value>
        VariableDefinition AroundInvokeProvider { get; }

        /// <summary>
        /// Gets the value indicating the local variable used to store the <see cref="IMethodReplacementProvider"/> instance.
        /// </summary>
        /// <value>The <see cref="IMethodReplacementProvider"/> instance.</value>
        VariableDefinition MethodReplacementProvider { get; }

        /// <summary>
        /// Gets the value indicating the class-level<see cref="IMethodReplacementProvider"/> instance.
        /// </summary>
        /// <value>The class-level<see cref="IMethodReplacementProvider"/> instance.</value>
        VariableDefinition ClassMethodReplacementProvider { get; }

        /// <summary>
        /// Gets the value indicating the local variable that will store the value that determines whether or not
        /// interception is disabled.
        /// </summary>
        /// <value>The value that determines whether or not interception is disabled.</value>
        VariableDefinition InterceptionDisabled { get; }

        /// <summary>
        /// Gets the value indicating the local variable that will store the <see cref="IInvocationInfo"/> instance.
        /// </summary>
        /// <value>The local variable that will store the <see cref="IInvocationInfo"/> instance.</value>
        VariableDefinition InvocationInfo { get; }

        /// <summary>
        /// Gets the value indicating the local variable that will store the return value.
        /// </summary>
        /// <value>The value indicating the local variable that will store the return value.</value>
        VariableDefinition ReturnValue { get; }

        /// <summary>
        /// Gets the value indicating the interception registry type that will be responsible for handling class-level interception events.
        /// </summary>
        /// <value>The interception registry type that will be responsible for handling class-level interception events.</value>
        Type RegistryType { get; }

        /// <summary>
        /// Gets the value indicating the functor that resolves the GetMethodReplacementProvider method.
        /// </summary>
        /// <value>The functor that resolves the GetMethodReplacementProvider method.</value>
        Func<ModuleDefinition, MethodReference> GetMethodReplacementProviderMethod { get; }

        /// <summary>
        /// Gets the value indicating the list of old instructions in the current method body.
        /// </summary>
        /// <value>The value indicating the list of old instructions in the current method body.</value>
        IEnumerable<Instruction> OldInstructions { get; }
    }
}