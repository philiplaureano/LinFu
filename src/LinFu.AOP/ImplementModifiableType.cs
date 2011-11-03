using System;
using System.Linq;
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
            bool shouldWeave = base.ShouldWeave(item);

            // Make sure that the IModifiableType interface is only implemented once
            shouldWeave &= !item.Interfaces.Any(typeReference => typeReference.FullName == _modifiableInterfaceType.FullName);

            bool isStaticClass = item.IsAbstract && item.IsSealed;
            shouldWeave &= !isStaticClass;

            return shouldWeave;
        }

        public override void Weave(TypeDefinition item)
        {
            base.Weave(item);

            // Implement IModifiableType
            if (item.Interfaces.Any(typeReference => typeReference.FullName == _modifiableInterfaceType.FullName))
                return;

            item.Interfaces.Add(_modifiableInterfaceType);
            item.AddProperty("IsInterceptionDisabled", typeof (bool));
            item.AddProperty("AroundMethodCallProvider", typeof (IAroundInvokeProvider));
            item.AddProperty("AroundMethodBodyProvider", typeof (IAroundInvokeProvider));
        }
    }
}