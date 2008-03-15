using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    public interface IArgumentHandler
    {
        void PushArguments(ParameterInfo[] parameters, ILGenerator IL, bool isStatic);
    }
}