using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a visitor class that can iterate over <see cref="MethodDefinition"/>
    /// instances.
    /// </summary>
    public class MethodWeaverVisitor : LinFu.AOP.Cecil.BaseReflectionVisitor
    {
        private readonly IMethodWeaver _methodWeaver;

        /// <summary>
        /// Initializes a new instance of the MethodWeaverVisitor class.
        /// </summary>
        /// <param name="methodWeaver">The <see cref="IMethodWeaver"/> that will be used to modify a given type.</param>
        public MethodWeaverVisitor(IMethodWeaver methodWeaver)
        {
            _methodWeaver = methodWeaver;
        }

        /// <summary>
        /// Visits a <see cref="MethodDefinition"/> instance.
        /// </summary>
        /// <param name="ctor">The <see cref="MethodDefinition"/> instance that will be modified.</param>
        public override void VisitConstructor(MethodDefinition ctor)
        {
            VisitMethodDefinition(ctor);
        }

        /// <summary>
        /// Visits a <see cref="MethodDefinition"/> instance.
        /// </summary>
        /// <param name="method">The <see cref="MethodDefinition"/> instance that will be modified.</param>
        public override void VisitMethodDefinition(MethodDefinition method)
        {
            if (!_methodWeaver.ShouldWeave(method))
                return;

            _methodWeaver.Weave(method);
            base.VisitMethodDefinition(method);
        }

        /// <summary>
        /// Visits a <see cref="ModuleDefinition"/> instance.
        /// </summary>
        /// <param name="module">A <see cref="ModuleDefinition"/> object.</param>
        public override void VisitModuleDefinition(ModuleDefinition module)
        {
            _methodWeaver.ImportReferences(module);
            base.VisitModuleDefinition(module);
        }

        /// <summary>
        /// Visits a <see cref="TypeDefinition"/> instance.
        /// </summary>
        /// <param name="type">A <see cref="TypeDefinition"/> object.</param>
        public override void VisitTypeDefinition(TypeDefinition type)
        {
            _methodWeaver.AddAdditionalMembers(type);
            base.VisitTypeDefinition(type);
        }
    }
}
