using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// Represents a class that is responsible for
    /// constructing method bodies.
    /// </summary>
    public interface IMethodBodyEmitter
    {
        /// <summary>
        /// Generates the method body for the <paramref name="targetMethod">target method</paramref>.
        /// </summary>
        /// <param name="originalMethod">The original method that the <paramref name="targetMethod"/> will be based on.</param>
        /// <param name="targetMethod">The method that will contain the method body to be emitted.</param>
        void Emit(MethodInfo originalMethod, MethodDefinition targetMethod);
    }
}
