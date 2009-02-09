using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.IoC.Configuration;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// A weaver class that automatically applies a <see cref="IHostWeaver{THost}"/> 
    /// instance to a target type definition.
    /// </summary>
    [Implements(typeof(ITypeWeaver), ServiceName = "AutoMethodWeaver")]
    internal class AutoTypeWeaver : ITypeWeaver
    {
        private readonly Action<MethodDefinition> _weave;
        private readonly IHostWeaver<TypeDefinition> _hostWeaver;

        /// <summary>
        /// Initializes the weaver so that it will apply the <paramref name="methodWeaver"/>
        /// to the target type during the <see cref="IWeaver{T,THost}.Weave"/> method call.
        /// </summary>
        /// <param name="methodWeaver">The <see cref="IMethodWeaver"/> that will be used to modify the target type.</param>
        public AutoTypeWeaver(IWeaver<MethodDefinition, TypeDefinition> methodWeaver)
        {
            _hostWeaver = methodWeaver;

            // Change the current method
            _weave = method =>
                         {
                             if (!methodWeaver.ShouldWeave(method))
                                 return;

                             methodWeaver.Weave(method);
                         };
        }


        /// <summary>
        /// Initializes the weaver so that it will apply the <paramref name="aroundWeaver"/>
        /// to the target type during the <see cref="IWeaver{T,THost}.Weave"/> method call.
        /// </summary>
        /// <param name="aroundWeaver">The method epilog/prolog weaver.</param>
        public AutoTypeWeaver(IAroundMethodWeaver aroundWeaver)
        {
            _hostWeaver = aroundWeaver;

            _weave = method =>
                         {
                             if (!aroundWeaver.ShouldWeave(method))
                                 return;

                             var body = method.Body;

                             var instructionCount = body.Instructions.Count;

                             // Skip empty method bodies
                             if (instructionCount == 0)
                                 return;

                             // Get the first instruction
                             var firstInstruction = body.Instructions[0];

                             // Get the last instruction
                             var lastIndex = body.Instructions.Count - 1;
                             var lastInstruction = body.Instructions[lastIndex];

                             aroundWeaver.AddProlog(firstInstruction, body);
                             aroundWeaver.AddEpilog(lastInstruction, body);
                         };
        }

        /// <summary>
        /// Determines whether or not the current type should be modified. 
        /// </summary>
        /// <param name="item">The target type to be modified.</param>
        /// <returns>This method always returns <c>true.</c></returns>
        public bool ShouldWeave(TypeDefinition item)
        {
            return true;
        }

        /// <summary>
        /// Modifies the target type definition using the given
        /// method weavers.
        /// </summary>
        /// <param name="item">The target type.</param>
        public void Weave(TypeDefinition item)
        {
            if (_weave == null || _hostWeaver == null)
                return;

            // Add additional members to the target item
            _hostWeaver.AddAdditionalMembers(item);

            foreach (MethodDefinition method in item.Methods)
            {
                _weave(method);
            }
        }

        /// <summary>
        /// Adds additional type references to the target <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The module definition that contains the target type definition.</param>
        public void ImportReferences(ModuleDefinition module)
        {
            if (_hostWeaver == null)
                return;

            _hostWeaver.ImportReferences(module);
        }

        /// <summary>
        /// Adds additional members to the target module definition.
        /// </summary>
        /// <remarks>This particular method implementation does absolutely nothing.</remarks>
        /// <param name="host">The target module definition.</param>
        public void AddAdditionalMembers(ModuleDefinition host)
        {
            // Do nothing
        }
    }
}
