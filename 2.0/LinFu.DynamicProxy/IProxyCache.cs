using System;

namespace LinFu.DynamicProxy
{
    public interface IProxyCache
    {
        bool Contains(Type baseType, params Type[] baseInterfaces);
        Type GetProxyType(Type baseType, params Type[] baseInterfaces);

        void StoreProxyType(Type result, Type baseType, params Type[] baseInterfaces);
    }
}