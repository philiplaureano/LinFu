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
    /// Represents the basic implementation of a method rewriter class.
    /// </summary>
    public abstract class BaseMethodRewriter : IMethodRewriter
    {
        private readonly HashSet<TypeDefinition> _modifiedTypes = new HashSet<TypeDefinition>();

        /// <summary>
        /// Initializes a new instance of the MethodRewriter class.
        /// </summary>
        protected BaseMethodRewriter() { }

        /// <summary>
        /// Rewrites a target method using the given CilWorker.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The CilWorker that will be used to rewrite the target method.</param>
        /// <param name="oldInstructions">The original instructions from the target method body.</param>
        public void Rewrite(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            if (!ShouldRewrite(method))
                return;

            var body = IL.GetBody();
            body.InitLocals = true;

            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            // Interfaces and Enums cannot be modified
            if (declaringType.IsInterface || declaringType.IsEnum)
                return;

            ImportReferences(module);

            AddLocals(method);

            if (!_modifiedTypes.Contains(declaringType))
            {
                AddAdditionalMembers(declaringType);
                _modifiedTypes.Add(declaringType);
            }

            RewriteMethodBody(method, IL, oldInstructions);
        }

        /// <summary>
        /// Determines whether or not the given method should be modified.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not a method should be rewritten.</returns>
        protected virtual bool ShouldRewrite(MethodDefinition targetMethod)
        {
            return true;
        }

        /// <summary>
        /// Rewrites the instructions in the target method body.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The <see cref="CilWorker"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected abstract void RewriteMethodBody(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions);
        

        /// <summary>
        /// Adds additional members to the host type.
        /// </summary>
        /// <param name="host">The host type.</param>
        public virtual void AddAdditionalMembers(TypeDefinition host)
        {
        }

        /// <summary>
        /// Adds additional references to the target module.
        /// </summary>
        /// <param name="module">The host module.</param>
        public virtual void ImportReferences(ModuleDefinition module)
        {
        }

        /// <summary>
        /// Adds local variables to the <paramref name="hostMethod"/>.
        /// </summary>
        /// <param name="hostMethod">The target method.</param>
        public virtual void AddLocals(MethodDefinition hostMethod)
        {
        }       
    }
}
