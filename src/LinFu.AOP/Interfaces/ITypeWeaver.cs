using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that can modify an existing <see cref="TypeDefinition"/>.
    /// </summary>
    public interface ITypeWeaver : IWeaver<TypeDefinition, ModuleDefinition>
    {
    }
}
