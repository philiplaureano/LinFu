using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Invariants
{
    public class TypeFilter
    {
        private ITypeContract _contract;
        private Predicate<Type> _predicate;
        public TypeFilter(ITypeContract contract)
        {
            _contract = contract;
        }

        public TypeFilter(ITypeContract contract, Predicate<Type> predicate)
        {
            _contract = contract;
            _predicate = predicate;
        }

        public ShowErrorAction IsAlwaysTrue<T>(Predicate<T> condition)
            where T : class
        {
            AdHocInvariant<object> invariant = new AdHocInvariant<object>();
            invariant.AppliesToHandler = delegate(object target, InvocationInfo info)
                                             {
                                                 if (target == null)
                                                     return false;
                                                 
                                                 Type targetType = target.GetType();
                                                 return _predicate(targetType);
                                             };

            invariant.CheckHandler =
                delegate(object target, InvocationInfo info, InvariantState callState)
                    {
                        return condition(target as T);
                    };
            
            _contract.Invariants.Add(invariant);

            return new ShowErrorAction(invariant, _contract);
        }
        public ShowErrorAction IsAlwaysFalse<T>(Predicate<T> condition)
            where T : class
        {
            AdHocInvariant<object> invariant = new AdHocInvariant<object>();
            invariant.AppliesToHandler = delegate(object target, InvocationInfo info)
                                             {
                                                 if (target == null)
                                                     return false;

                                                 Type targetType = target.GetType();
                                                 return _predicate(targetType);
                                             };

            invariant.CheckHandler =
                delegate(object target, InvocationInfo info, InvariantState callState)
                {
                    return !condition(target as T);
                };

            _contract.Invariants.Add(invariant);

            return new ShowErrorAction(invariant, _contract);
        }
        public ImplyCondition<T> HavingCondition<T>(Predicate<T> condition)
            where T : class
        {
            AppliesToHandler appliesTo = delegate(object target, InvocationInfo info)
                                             {
                                                 if (target == null)
                                                     return false;

                                                 Type targetType = target.GetType();
                                                 return _predicate(targetType);
                                             };

            ImplyCondition<T> implication = new ImplyCondition<T>(_contract, appliesTo, condition);

            return implication;
        }
    }
}
