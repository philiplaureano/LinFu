using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;
using LinFu.Reflection;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public class PropertyComparison<T>
    {
        private string _propertyName;
        private IMethodContract _contract;
        public PropertyComparison(string propertyName, IMethodContract contract)
        {
            _propertyName = propertyName;
            _contract = contract;
        }
        public ShowErrorAction ShouldNotBe<T>(PropertyComparisonHandler<T> predicate)
        {
            PropertyComparisonHandler<T> inverse = delegate(T oldValue, T newValue)
                                                       {
                                                           return !predicate(oldValue, newValue);
                                                       };

            return ShouldBe(inverse);
        }
        public ShowErrorAction ShouldBe<T>(PropertyComparisonHandler<T> predicate)
        {
            AdHocPostcondition<object> adHoc = new AdHocPostcondition<object>();
            adHoc.SaveProperty(_propertyName);

            adHoc.Checker = delegate(object target, InvocationInfo info, object returnValue)
                                             {
                                                 if (target == null)
                                                     return true;

                                                 DynamicObject dynamic = new DynamicObject(target);
                                                 T newValue = (T)dynamic.Properties[_propertyName];
                                                 T oldValue = (T)adHoc.GetOldValue(_propertyName);

                                                 return predicate(oldValue, newValue);
                                             };
            return new ShowErrorAction(adHoc, _contract);
        }
    }
}
