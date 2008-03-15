using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Attributes;

namespace LinFu.DesignByContract2.Injectors
{
    public interface IContractStorage
    {
        bool HasContractFor(Type targetType);
        IContractSource GetContractTypeFor(Type targetType);
        void AddContractType(Type targetType, IContractSource contractSourceType);
    }
}
