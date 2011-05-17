﻿using System.Collections.Generic;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Provides the basic functionality for the <see cref="IMethodRewriter"/> interface.
    /// </summary>
    public abstract class MethodRewriter : IMethodRewriter
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
            TypeDefinition declaringType = method.DeclaringType;
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

            var newInstructions = new Queue<Instruction>();
            foreach (Instruction instruction in oldInstructions)
            {
                // Intercept only the load field and the load static field instruction
                if (!ShouldReplace(instruction, method))
                {
                    IL.Append(instruction);
                    continue;
                }

                Replace(instruction, method, IL);
            }
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
        /// Determines whether or not the method rewriter should replace the <paramref name="oldInstruction"/>.
        /// </summary>
        /// <param name="oldInstruction">The instruction that is currently being evaluated.</param>
        /// <param name="hostMethod">The method that hosts the current instruction.</param>
        /// <returns><c>true</c> if the method should be replaced; otherwise, it should return <c>false</c>.</returns>
        protected abstract bool ShouldReplace(Instruction oldInstruction, MethodDefinition hostMethod);


        /// <summary>
        /// Replaces the <paramref name="oldInstruction"/> with a new set of <paramref name="IL"/> instructions..
        /// </summary>
        /// <param name="oldInstruction">The instruction currently being evaluated.</param>
        /// <param name="hostMethod">The method that contains the target instruction.</param>
        /// <param name="IL">The ILProcessor for the target method body.</param>
        protected abstract void Replace(Instruction oldInstruction, MethodDefinition hostMethod, ILProcessor IL);
    }
}