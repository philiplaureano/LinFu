using System;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    internal class ImplementMethodReplacementHost : ITypeWeaver
    {
        private readonly Func<TypeReference, bool> _filter;
        private TypeReference _hostType;

        public ImplementMethodReplacementHost(Func<TypeReference, bool> filter)
        {
            _filter = filter;
        }

        #region ITypeWeaver Members

        public virtual bool ShouldWeave(TypeDefinition item)
        {
            if (item.Name == "<Module>")
                return false;

            if (!item.IsClass || item.IsInterface)
                return false;

            // Implement the host interface once and only once
            if (item.Interfaces.Contains(_hostType))
                return false;

            if (_filter != null)
                return _filter(item);

            return true;
        }

        public virtual void Weave(TypeDefinition item)
        {
            if (item.Interfaces.Contains(_hostType))
                return;

            item.Interfaces.Add(_hostType);
            item.AddProperty("MethodBodyReplacementProvider", typeof (IMethodReplacementProvider));
            item.AddProperty("MethodCallReplacementProvider", typeof (IMethodReplacementProvider));
        }

        public virtual void AddAdditionalMembers(ModuleDefinition host)
        {
        }

        public virtual void ImportReferences(ModuleDefinition module)
        {
            _hostType = module.Import(typeof (IMethodReplacementHost));
        }

        #endregion
    }
}