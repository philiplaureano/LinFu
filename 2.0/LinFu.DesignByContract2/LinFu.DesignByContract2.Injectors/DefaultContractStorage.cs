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
        private readonly Dictionary<Type, IContractSource> _contractTypes = new Dictionary<Type, IContractSource>();
        private readonly object lockObject = new object();
        #region IContractStorage Members

        public bool HasContractFor(Type targetType)
        {
            bool result;

            lock (lockObject)
            {
                result = _contractTypes.ContainsKey(targetType);
            }
            return result;
        }

        public IContractSource GetContractTypeFor(Type targetType)
        {
            IContractSource result = null;
            lock (lockObject)
            {
                result = _contractTypes[targetType];
            }
            return result;
        }

        public void AddContractType(Type targetType, IContractSource contractSourceType)
        {
            lock (lockObject)
            {
                _contractTypes[targetType] = contractSourceType;
            }
        }

        #endregion
    }
}
