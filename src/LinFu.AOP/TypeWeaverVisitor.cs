using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a visitor class that can iterate over <see cref="TypeDefinition"/>
    /// instances.
    /// </summary>
    public class TypeWeaverVisitor : LinFu.AOP.Cecil.BaseReflectionVisitor
    {
        private ITypeWeaver _weaver;
        private HashSet<ModuleDefinition> _visitedModules = new HashSet<ModuleDefinition>();

        /// <summary>
        /// Initializes a new instance of the TypeWeaverVisitor class.
        /// </summary>
        /// <param name="weaver">The <see cref="ITypeWeaver"/> that will be used to modify a given type.</param>
        public TypeWeaverVisitor(ITypeWeaver weaver)
        {
            _weaver = weaver;
        }

        /// <summary>
        /// Visits a <see cref="TypeDefinition"/> instance.
        /// </summary>
        /// <param name="type">A <see cref="TypeDefinition"/> object.</param>
        public override void VisitTypeDefinition(TypeDefinition type)
        {
            if (type.IsEnum)
                return;

            if (!_weaver.ShouldWeave(type))
                return;

            var module = type.Module;
            if (!_visitedModules.Contains(module))
            {
                _weaver.ImportReferences(module);
                _weaver.AddAdditionalMembers(module);
                _visitedModules.Add(module);
            }

            _weaver.Weave(type);

            base.VisitTypeDefinition(type);
        }
    }
}
