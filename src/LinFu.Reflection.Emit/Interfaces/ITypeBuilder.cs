using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.Reflection.Emit.Interfaces
{
    /// <summary>
    /// Represents a type that can construct <see cref="TypeDefinition"/>
    /// types.
    /// </summary>
    public interface ITypeBuilder
    {
        /// <summary>
        /// Constructs a <paramref name="targetType"/> using
        /// the given <see cref="ModuleDefinition"/> instance.
        /// </summary>
        /// <param name="module">The module that will hold the actual type.</param>
        /// <param name="targetType">The type being constructed.</param>
        void Construct(ModuleDefinition module, TypeDefinition targetType);
    }
}
