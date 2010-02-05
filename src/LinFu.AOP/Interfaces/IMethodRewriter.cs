using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that can modify method bodies.
    /// </summary>
    public interface IMethodRewriter : IMethodBodyRewriter, IHostWeaver<TypeDefinition>
    {
        /// <summary>
        /// Adds local variables to the <paramref name="hostMethod"/>.
        /// </summary>
        /// <param name="hostMethod">The target method.</param>
        void AddLocals(MethodDefinition hostMethod);        
    }
}
