using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public class ParameterFilter
    {
        private AdHocPrecondition<object> _precondition;
        private Predicate<ParameterInfo> _parameterTest;
        private IMethodContract _contract;

        public ParameterFilter(AdHocPrecondition<object> precondition, IMethodContract contract, Predicate<ParameterInfo> parameterTest)
        {
            _precondition = precondition;
            _parameterTest = parameterTest;
            _contract = contract;
        }

        public ShowParameterErrorAction ShouldBe<T>(Predicate<T> condition)            
        {
            _precondition.CheckHandler = delegate(object target, InvocationInfo info)
                                             {
                                                 ParameterInfo[] parameters = info.TargetMethod.GetParameters();

                                                 // Search for a compatible parameter

                                                 int targetPosition = -1;
                                                 foreach (ParameterInfo param in parameters)
                                                 {
                                                     // Search for a matching parameter
                                                     if (!_parameterTest(param) && param.ParameterType.IsAssignableFrom(typeof(T)))
                                                         continue;

                                                     targetPosition = param.Position;
                                                     break;
                                                 }

                                                 if (targetPosition <= 0)
                                                     return false;

                                                 T parameterValue = (T)info.Arguments[targetPosition];
                                                 return condition(parameterValue);
                                             };

            ShowParameterErrorAction action = new ShowParameterErrorAction(_contract, _precondition);
            return action;
        }
    }
}
