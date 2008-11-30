using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that emits
    /// the IL to save information about
    /// the method currently being executed.
    /// </summary>
    public interface IEmitInvocationInfo
    {
        /// <summary>
        /// Emits the IL to save information about
        /// the method currently being executed.
        /// </summary>
        /// <seealso cref="IInvocationInfo"/>
        /// <param name="targetMethod">The target method currently being executed.</param>
        /// <param name="currentMethod">The method that will be passed to the <paramref name="invocationInfo"/> as the currently executing method.</param>
        /// <param name="invocationInfo">The local variable that will store the resulting <see cref="IInvocationInfo"/> instance.</param>
        void Emit(MethodInfo currentMethod, MethodDefinition targetMethod, 
            VariableDefinition invocationInfo);
    }
}
