using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Attributes;
using Simple.IoC.Loaders;

namespace LinFu.DesignByContract2.Injectors
{
    [Implements(typeof(IContractStorage), LifecycleType.Singleton)]
    public class DefaultContractStorage : IContractStorage 
    {
        private Dictionary<Type, IContractSource> _contractTypes = new Dictionary<Type, IContractSource>();
        #region IContractStorage Members

        public bool HasContractFor(Type targetType)
        {
            return _contractTypes.ContainsKey(targetType);
        }

        public IContractSource GetContractTypeFor(Type targetType)
        {
            return _contractTypes[targetType];
        }

        public void AddContractType(Type targetType, IContractSource contractSourceType)
        {
            _contractTypes[targetType] = contractSourceType;
        }

        #endregion
    }
}
