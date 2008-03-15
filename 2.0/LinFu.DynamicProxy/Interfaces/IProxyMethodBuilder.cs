using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    public interface IProxyMethodBuilder
    {
        void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder);
    }
}