using System.IO;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public interface IPostcondition : IMethodContractCheck
    {
        void BeforeMethodCall(object target, InvocationInfo info);
        bool Check(object target, InvocationInfo info, object returnValue);
        void ShowError(TextWriter output, object target, InvocationInfo info, object returnValue);
    }
}
