using System;
using System.IO;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public interface IPrecondition : IMethodContractCheck
    {
        bool Check(object target, InvocationInfo info);
        void ShowError(TextWriter output, object target, InvocationInfo info);
    }
}
