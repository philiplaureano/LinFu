using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// Represents a class that generates methods based on other existing
    /// methods.
    /// </summary>
    public interface IMethodBuilder
    {
        /// <summary>
        /// Creates a method that matches the signature defined in the
        /// <paramref name="method"/> parameter.
        /// </summary>
        /// <param name="targetType">The type that will host the new method.</param>
        /// <param name="method">The method from which the signature will be derived.</param>
        MethodDefinition CreateMethod(TypeDefinition targetType, MethodInfo method);
    }
}
