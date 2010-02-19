using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using LinFu.AOP.Cecil.Interfaces;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    /// A helper class that extends Cecil to support LinFu's weaver model.
    /// </summary>
    public static class CecilVisitorExtensions
    {
        /// <summary>
        /// Allows a <see cref="ITypeWeaver"/> instance to traverse any <see cref="IReflectionVisitable"/>
        /// instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="typeWeaver">The type weaver.</param>
        public static void Accept(this IReflectionVisitable visitable, ITypeWeaver typeWeaver)
        {
            var visitor = new TypeWeaverVisitor(typeWeaver);
            visitable.Accept(visitor);
        }

        /// <summary>
        /// Allows a <see cref="ITypeWeaver"/> instance to traverse any <see cref="IReflectionStructureVisitable"/>
        /// instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="typeWeaver">The type weaver.</param>
        public static void Accept(this IReflectionStructureVisitable visitable, ITypeWeaver typeWeaver)
        {
            var visitor = new TypeWeaverVisitor(typeWeaver);
            visitable.Accept(visitor);
        }

        /// <summary>
        /// Allows a <see cref="IMethodWeaver"/> instance to traverse any <see cref="IReflectionVisitable"/>
        /// instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="methodWeaver">The method weaver.</param>
        public static void Accept(this IReflectionStructureVisitable visitable, IMethodWeaver methodWeaver)
        {
            var visitor = new MethodWeaverVisitor(methodWeaver);
            visitable.Accept(visitor);
        }

        /// <summary>
        /// Allows a <see cref="IMethodWeaver"/> instance to traverse any <see cref="IReflectionVisitable"/>
        /// instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="methodWeaver">The method weaver.</param>
        public static void Accept(this IReflectionVisitable visitable, IMethodWeaver methodWeaver)
        {
            var visitor = new MethodWeaverVisitor(methodWeaver);
            visitable.Accept(visitor);
        }
    }
}