using System;
using System.IO;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public interface IInvariant : IContractCheck
    {
        bool Check(object target, InvocationInfo info, InvariantState callState);
        void ShowError(TextWriter output, object target, InvocationInfo info, InvariantState callState);
    }
}
