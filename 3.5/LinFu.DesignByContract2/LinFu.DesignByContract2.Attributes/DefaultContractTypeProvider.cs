using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Attributes
{
    public class DefaultContractTypeProvider : IContractTypeProvider 
    {
        #region IContractTypeProvider Members

        public IContractSource ProvideContractForType(Type targetType)
        {
            return new TypeContractSource(targetType);
        }

        #endregion
    }
}
