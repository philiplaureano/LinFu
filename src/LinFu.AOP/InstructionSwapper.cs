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
    /// Provides the basic functionality for the <see cref="IMethodRewriter"/> interface.
    /// </summary>
    public abstract class InstructionSwapper : BaseMethodRewriter
    {
        /// <summary>
        /// Initializes a new instance of the MethodRewriter class.
        /// </summary>
        protected InstructionSwapper() { }
        
        protected override void RewriteMethodBody(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var newInstructions = new Queue<Instruction>();
            foreach (var instruction in oldInstructions)
            {
                if (!ShouldReplace(instruction, method))
                {
                    IL.Append(instruction);
                    continue;
                }

                Replace(instruction, method, IL);
            }
        }        

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
        /// <param name="IL">The CilWorker for the target method body.</param>
        protected abstract void Replace(Instruction oldInstruction, MethodDefinition hostMethod, CilWorker IL);
    }
}
