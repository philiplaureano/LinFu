using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public interface IContractCheck
    {
        bool AppliesTo(object target, InvocationInfo info);
        void Catch(Exception ex);
    }
}
