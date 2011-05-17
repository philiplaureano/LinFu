using System.Collections.Generic;
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

        #region IMethodRewriter Members

        /// <summary>
        /// Rewrites a target method using the given ILProcessor.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The ILProcessor that will be used to rewrite the target method.</param>
        /// <param name="oldInstructions">The original instructions from the target method body.</param>
        public void Rewrite(MethodDefinition method, ILProcessor IL, IEnumerable<Instruction> oldInstructions)
        {
            if (!ShouldRewrite(method))
                return;

            TypeDefinition declaringType = method.DeclaringType;

            MethodBody body = IL.Body;
            body.InitLocals = true;

            ModuleDefinition module = declaringType.Module;

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

        #endregion

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
        /// <param name="IL">The <see cref="ILProcessor"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected abstract void RewriteMethodBody(MethodDefinition method, ILProcessor IL,
                                                  IEnumerable<Instruction> oldInstructions);
    }
}