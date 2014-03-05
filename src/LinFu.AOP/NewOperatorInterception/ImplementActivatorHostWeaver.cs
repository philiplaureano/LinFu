using System;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    internal class ImplementActivatorHostWeaver : ITypeWeaver
    {
        private readonly Func<TypeReference, bool> _filter;
        private TypeReference _activatorPropertyType;
        private TypeReference _hostInterfaceType;

        public ImplementActivatorHostWeaver()
            : this(type => true)
        {
        }

        public ImplementActivatorHostWeaver(Func<TypeReference, bool> filter)
        {
            _filter = filter;
        }


        public bool ShouldWeave(TypeDefinition item)
        {
            if (item.Name == "<Module>")
                return false;

            if (!item.IsClass || item.IsInterface)
                return false;

            if (_filter != null)
                return _filter(item);

            return true;
        }

        public void Weave(TypeDefinition type)
        {
            // Implement IActivatorHost only once
            if (type.Interfaces.Contains(_hostInterfaceType))
                return;

            type.AddProperty("Activator", _activatorPropertyType);

            if (!type.Interfaces.Contains(_hostInterfaceType))
                type.Interfaces.Add(_hostInterfaceType);
        }

        public void AddAdditionalMembers(ModuleDefinition module)
        {
            _hostInterfaceType = module.ImportType<IActivatorHost>();
            _activatorPropertyType = module.ImportType<ITypeActivator>();
        }

        public void ImportReferences(ModuleDefinition module)
        {
        }
    }
}