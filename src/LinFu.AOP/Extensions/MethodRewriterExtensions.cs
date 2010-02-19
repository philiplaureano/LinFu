using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using LinFu.AOP.Cecil.Interfaces;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// A helper class that extends Cecil to support the <see cref="IMethodRewriter"/> interface.
    /// </summary>
    public static class MethodRewriterExtensions
    {
        /// <summary>
        /// Transforms the methods in the <paramref name="target"/> using the given method rewriter.
        /// </summary>
        /// <param name="target">The transformation target.</param>
        /// <param name="rewriter">The method rewriter.</param>
        /// <param name="filter">The method filter that determines which methods will be rewritten.</param>
        public static void WeaveWith(this IReflectionStructureVisitable target, IMethodRewriter rewriter, Func<MethodReference, bool> filter)
        {
            var weaver = new MethodWeaver(rewriter, filter);
            target.Accept(weaver);
        }

        /// <summary>
        /// Transforms the methods in the <paramref name="target"/> using the given method rewriter.
        /// </summary>
        /// <param name="target">The transformation target.</param>
        /// <param name="rewriter">The method rewriter.</param>
        /// <param name="filter">The method filter that determines which methods will be rewritten.</param>
        public static void WeaveWith(this IReflectionVisitable target, IMethodRewriter rewriter, Func<MethodReference, bool> filter)
        {
            var weaver = new MethodWeaver(rewriter, filter);
            target.Accept(weaver);
        }
    }
}
