using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// A class that verifies a given <see cref="AssemblyDefinition"/> instance.
    /// </summary>
    public interface IVerifier
    {
        /// <summary>
        /// Verifies the given <paramref name="assembly"/> instance.
        /// </summary>
        /// <param name="assembly">The assembly definition that needs to be verified.</param>
        void Verify(AssemblyDefinition assembly);
    }
}