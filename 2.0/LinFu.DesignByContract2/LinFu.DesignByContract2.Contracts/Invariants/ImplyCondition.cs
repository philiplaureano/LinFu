using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Invariants
{
    public class ImplyCondition<T>
        where T : class 
    {
        private ITypeContract _contract;
        private AppliesToHandler _appliesTo;
        private Predicate<T> _condition;

        public ImplyCondition(ITypeContract contract, AppliesToHandler appliesTo, Predicate<T> condition)
        {
            _contract = contract;
            _condition = condition;
            _appliesTo = appliesTo;
        }

        public ShowErrorAction ImpliesThat(Predicate<T> predicate)
        {
            AdHocInvariant<object> invariant = new AdHocInvariant<object>();
            invariant.AppliesToHandler = _appliesTo;
            invariant.CheckHandler = delegate(object target, InvocationInfo info, InvariantState callState)
                {

                    if (_condition(target as T) == true)
                        return predicate(target as T);
                                             
                    return false;
                };
            
            _contract.Invariants.Add(invariant);

            ShowErrorAction result = new ShowErrorAction(invariant, _contract);

            return result;
        }
    }
}
