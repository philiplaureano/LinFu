using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IMethodWeaver"/> interface.
    /// </summary>
    public class MethodWeaver : IMethodWeaver
    {
        private readonly Func<MethodReference, bool> _filter;
        private readonly IMethodRewriter _rewriter;
        private readonly HashSet<MethodReference> _visitedMethods = new HashSet<MethodReference>();
        private readonly IInstructionProvider _instructionProvider;

        /// <summary>
        /// Initializes a new instance of the MethodWeaver class.
        /// </summary>
        /// <param name="rewriter">The <see cref="IMethodRewriter"/> instance that will modify the existing method.</param>
        /// <param name="filter">The filter that determines which methods should be modified.</param>        
        public MethodWeaver(IMethodRewriter rewriter, Func<MethodReference, bool> filter)
            : this(rewriter, new InstructionProvider(), filter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MethodWeaver class.
        /// </summary>
        /// <param name="rewriter">The <see cref="IMethodRewriter"/> instance that will modify the existing method.</param>
        /// <param name="instructionProvider">The provider that will obtain the original instructions for the target method.</param>
        /// <param name="filter">The filter that determines which methods should be modified.</param>        
        public MethodWeaver(IMethodRewriter rewriter, IInstructionProvider instructionProvider, Func<MethodReference, bool> filter)
        {
            _filter = filter;
            _rewriter = rewriter;
            _instructionProvider = instructionProvider;
        }

        /// <summary>
        /// Determines whether or not a method should be modified.
        /// </summary>
        /// <param name="item">The target method.</param>
        /// <returns><c>true</c> if the method should be modified; otherwise, it returns <c>false</c>.</returns>
        public bool ShouldWeave(MethodDefinition item)
        {
            if (_visitedMethods.Contains(item))
                return false;

            if (_rewriter == null)
                return false;

            if (!_filter(item))
                return false;

            return !item.IsAbstract;
        }

        /// <summary>
        /// Modifies a target method.
        /// </summary>
        /// <param name="method">The target method.</param>
        public void Weave(MethodDefinition method)
        {
            var body = method.Body;
            var IL = body.CilWorker;

            // Skip empty methods
            var instructionCount = body.Instructions.Count;
            if (instructionCount == 0)
                return;

            Rewrite(method);

            _visitedMethods.Add(method);
        }

        /// <summary>
        /// Rewrites an existing method.
        /// </summary>
        /// <param name="method">The method that needs to be modified.</param>
        private void Rewrite(MethodDefinition method)
        {
            var body = method.Body;
            var IL = body.CilWorker;

            var oldInstructions = _instructionProvider.GetInstructions(method);

            body.Instructions.Clear();

            _rewriter.AddLocals(method);
            _rewriter.Rewrite(method, IL, oldInstructions);
        }

        /// <summary>
        /// Adds additional members to the target type.
        /// </summary>
        /// <param name="host">The target type to be modified.</param>
        public void AddAdditionalMembers(TypeDefinition host)
        {
            if (_rewriter == null)
                return;

            _rewriter.AddAdditionalMembers(host);
        }

        /// <summary>
        /// Imports additional references into the given module.
        /// </summary>
        /// <param name="module">The module that will store the additional references.</param>
        public void ImportReferences(ModuleDefinition module)
        {
            if (_rewriter == null)
                return;

            _rewriter.ImportReferences(module);
        }
    }
}
