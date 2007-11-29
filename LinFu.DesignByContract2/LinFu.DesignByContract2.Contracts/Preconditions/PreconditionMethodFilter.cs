using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public class PreconditionMethodFilter
    {
        private AdHocPrecondition<object> _precondition;
        private IMethodContract _contract;
        public PreconditionMethodFilter(AdHocPrecondition<object> precondition, IMethodContract contract)
        {
            _precondition = precondition;
            _contract = contract;
        }
        public ShowErrorAction That<T>(Predicate<T> check)
            where T : class
        {
            _precondition.CheckHandler = delegate(object target, InvocationInfo info)
                                             {
                                                 T targetObject = target as T;
                                                 if (targetObject == null || check == null)
                                                     return true;

                                                 return check(targetObject);
                                             };

            ShowErrorAction action = new ShowErrorAction(_precondition, _contract);
            return action;
        }
    }
}
