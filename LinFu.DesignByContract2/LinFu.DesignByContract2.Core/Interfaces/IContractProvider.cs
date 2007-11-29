using System;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public interface IContractProvider
    {
        ITypeContract GetTypeContract(Type targetType);
        IMethodContract GetMethodContract(Type targetType, InvocationInfo info);
    }
}