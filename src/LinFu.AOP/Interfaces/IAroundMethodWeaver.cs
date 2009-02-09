using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a class that adds a method
    /// epilog and prolog to an existing method instance.
    /// </summary>
    public interface IAroundMethodWeaver : IHostWeaver<TypeDefinition>
    {
        /// <summary>
        /// Determines whether or not the current item should be modified.
        /// </summary>
        /// <param name="method">The target item.</param>
        /// <returns>Returns <c>true</c> if the current item can be modified; otherwise, it should return <c>false.</c></returns>
        bool ShouldWeave(MethodDefinition method);

        /// <summary>
        /// Adds an prolog to the target method.
        /// </summary>
        /// <param name="firstInstruction">The instruction that marks the start of the <paramref name="methodBody"/></param>
        /// <param name="methodBody">The method body of the target method.</param>
        void AddProlog(Instruction firstInstruction, MethodBody methodBody);

        /// <summary>
        /// Adds an epilog to the target method.
        /// </summary>
        /// <param name="lastInstruction">The instruction that marks the end of the <paramref name="methodBody"/></param>
        /// <param name="methodBody">The method body of the target method.</param>
        void AddEpilog(Instruction lastInstruction, MethodBody methodBody);
    }
}
