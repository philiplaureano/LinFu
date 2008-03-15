using System;

namespace LinFu.DynamicProxy
{
    public interface IProxyCache
    {
        bool Contains(Type baseType, Type[] interfaces);
        Type GetProxyType(Type baseType, Type[] interfaces);

        void StoreProxyType(Type result, Type baseType, Type[] baseInterfaces);
    }
}