using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public class PostconditionMethodFilter
    {
        private readonly AdHocPostcondition<object> _postcondition;
        private readonly IMethodContract _contract;
        public PostconditionMethodFilter(AdHocPostcondition<object> postcondition, IMethodContract contract)
        {
            _postcondition = postcondition;
            _contract = contract;
        }
        public ShowErrorAction That<T>(Predicate<T> check)
            where T : class
        {
            _postcondition.Checker = delegate(object target, InvocationInfo info, object returnValue)
                                             {
                                                 T targetObject = target as T;
                                                 if (targetObject == null || check == null)
                                                     return true;

                                                 return check(targetObject);
                                             };

            ShowErrorAction action = new ShowErrorAction(_postcondition, _contract);
            return action;
        }
        public ShowErrorAction ReturnValueIs<T>(Predicate<T> check)
        {
            _postcondition.Checker = delegate(object target, InvocationInfo info, object returnValue)
                                             {
                                                 T result = (T)returnValue;

                                                 return check(result);
                                             };

            ShowErrorAction action = new ShowErrorAction(_postcondition, _contract);
            return action;
        }
        public PropertyNameFilter<T> ThatProperty<T>(string propertyName)
        {
            PropertyNameFilter<T> nameFilter = new PropertyNameFilter<T>(propertyName, _contract);
            return nameFilter;
        }
    }
}
