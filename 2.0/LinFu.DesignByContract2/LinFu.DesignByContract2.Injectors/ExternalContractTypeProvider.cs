using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Attributes;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.DesignByContract2.Injectors
{
    [Implements(typeof(IContractTypeProvider), LifecycleType.OncePerRequest)]
    public class ExternalContractTypeProvider : IContractTypeProvider, IInitialize
    {
        private IContractStorage _contractStorage;

        public IContractStorage ContractStorage
        {
            get { return _contractStorage; }
            set { _contractStorage = value; }
        }

        #region IContractTypeProvider Members

        public IContractSource ProvideContractForType(Type targetType)
        {
            // Scan the type itself for the contract assertions
            if (!_contractStorage.HasContractFor(targetType))
                return new TypeContractSource(targetType);
            
            // Use the external contract if it exists
            return _contractStorage.GetContractTypeFor(targetType);
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _contractStorage = container.GetService<IContractStorage>();
        }

        #endregion
    }
}
