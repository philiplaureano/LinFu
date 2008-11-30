using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// Represents a class that is responsible
    /// for generating proxy types.
    /// </summary>
    public interface IProxyBuilder
    {
        /// <summary>
        /// Generates a proxy that must be derived
        /// from the <paramref name="baseType"/> and implement
        /// the list of <paramref name="interfaces"/>.
        /// </summary>
        /// <param name="baseType">The base class of the type being constructed.</param>
        /// <param name="interfaces">The list of interfaces that the new type must implement.</param>
        /// <param name="module">The module that will hold the brand new type.</param>
        /// <param name="targetType">The <see cref="TypeDefinition"/> that represents the type to be created.</param>
        void Construct(Type baseType, IEnumerable<Type> interfaces,
                       ModuleDefinition module, TypeDefinition targetType);
        
    }
}
