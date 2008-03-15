using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Attributes
{
    public interface IContractTypeProvider
    {
        IContractSource ProvideContractForType(Type targetType);
    }
}
