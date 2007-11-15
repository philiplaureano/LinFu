using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    public interface IMethodBodyEmitter
    {
        void EmitMethodBody(ILGenerator IL, MethodInfo method,
                            FieldInfo field);
    }
}