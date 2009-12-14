using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    internal class ImplementModifiableType : ImplementMethodReplacementHost
    {
        private TypeReference _modifiableInterfaceType;
        public ImplementModifiableType(Func<TypeReference, bool> filter)
            : base(filter)
        {
        }

        public override void ImportReferences(ModuleDefinition module)
        {
            // Import the references from the base class
            base.ImportReferences(module);

            _modifiableInterfaceType = module.ImportType<IModifiableType>();            
        }

        public override bool ShouldWeave(TypeDefinition item)
        {
            var shouldWeave = base.ShouldWeave(item);

            // Make sure that the IModifiableType interface is only implemented once
            shouldWeave &= !item.Interfaces.Contains(_modifiableInterfaceType);

            var isStaticClass = item.IsAbstract && item.IsSealed;
            shouldWeave &= !isStaticClass;

            return shouldWeave;
        }

        public override void Weave(TypeDefinition item)
        {
            base.Weave(item);

            // Implement IModifiableType
            item.Interfaces.Add(_modifiableInterfaceType);
            item.AddProperty("IsInterceptionDisabled", typeof(bool));
            item.AddProperty("AroundInvokeProvider", typeof(IAroundInvokeProvider));            
        }
    }
}
