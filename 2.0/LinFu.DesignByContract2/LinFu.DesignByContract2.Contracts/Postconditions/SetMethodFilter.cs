using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public class SetMethodFilter
    {
        private IMethodContract _contract;

        public SetMethodFilter(IMethodContract contract)
        {
            _contract = contract;
        }
        public PostconditionMethodFilter ForMethodWith(Predicate<MethodInfo> predicate)
        {
            AdHocPostcondition<object> adHoc = new AdHocPostcondition<object>();
            adHoc.AppliesToHandler = delegate(object target, InvocationInfo info)
                                      {
                                          return predicate(info.TargetMethod);
                                      };

            PostconditionMethodFilter filter = new PostconditionMethodFilter(adHoc, _contract);
            return filter;
        }      
    }
}
