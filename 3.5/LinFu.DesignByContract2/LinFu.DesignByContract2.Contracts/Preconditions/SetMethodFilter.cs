using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public class SetMethodFilter
    {
        private IMethodContract _contract;

        public SetMethodFilter(IMethodContract contract)
        {
            _contract = contract;
        }
        public PreconditionMethodFilter ForMethodWith(Predicate<MethodInfo> predicate)
        {
            AdHocPrecondition<object> adHoc = new AdHocPrecondition<object>();
            adHoc.AppliesToHandler = delegate(object target, InvocationInfo info)
                                      {
                                          return predicate(info.TargetMethod);
                                      };

            PreconditionMethodFilter filter = new PreconditionMethodFilter(adHoc, _contract);
            return filter;
        }
    }
}
