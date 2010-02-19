using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using LinFu.AOP.Interfaces;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a type weaver that modifies types to implement the <see cref="IFieldInterceptionHost"/> interface.
    /// </summary>
    public class ImplementFieldInterceptionHostWeaver : ITypeWeaver
    {
        private Func<TypeReference, bool> _filter;
        private TypeReference _hostInterfaceType;
        private TypeReference _interceptorPropertyType;

        /// <summary>
        /// Initializes a new instance of the ImplementFieldInterceptionHostWeaver class.
        /// </summary>
        /// <param name="filter">The filter that determines which types should be modified.</param>
        public ImplementFieldInterceptionHostWeaver(Func<TypeReference, bool> filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// Determines whether or not a type should be modified.
        /// </summary>
        /// <param name="item"></param>
        /// <returns><c>true</c> if the type should be modified; otherwise, it will return <c>false</c>.</returns>
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

        /// <summary>
        /// Modifies the target type.
        /// </summary>
        /// <param name="type">The type to be modified.</param>
        public void Weave(TypeDefinition type)
        {
            // Implement IActivatorHost only once
            if (type.Interfaces.Contains(_hostInterfaceType))
                return;

            type.AddProperty("FieldInterceptor", _interceptorPropertyType);

            if (!type.Interfaces.Contains(_hostInterfaceType))
                type.Interfaces.Add(_hostInterfaceType);
        }

        /// <summary>
        /// Adds additional members to the target module.
        /// </summary>
        /// <param name="host">The host module.</param>
        public void AddAdditionalMembers(ModuleDefinition host)
        {
        }

        /// <summary>
        /// Imports references into the target module.
        /// </summary>
        /// <param name="module">The module containing the type to be modified.</param>
        public void ImportReferences(ModuleDefinition module)
        {
            _hostInterfaceType = module.ImportType<IFieldInterceptionHost>();
            _interceptorPropertyType = module.ImportType<IFieldInterceptor>();
        }
    }
}
