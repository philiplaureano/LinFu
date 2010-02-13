using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a type that determines whether or not a particular type should be modified.
    /// </summary>
    public interface ITypeFilter
    {
        /// <summary>
        /// Determines whether or not a type should be modified.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>Returns <c>true</c> if the type should be modified.</returns>
        bool ShouldWeave(TypeDefinition type);
    }
}