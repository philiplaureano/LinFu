using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;

namespace LinFu.DesignByContract2.Contracts.Invariants
{
    public class SetTypeContract
    {
        private ITypeContract _contract;
        public SetTypeContract(ITypeContract contract)
        {
            _contract = contract;   
        }
        public TypeFilter WhereTypeIs(Predicate<Type> predicate)
        {
            TypeFilter filter = new TypeFilter(_contract, predicate);
            return filter;
        }
        public TypeFilter WhereTypeIs(Type type)
        {
            return WhereTypeIs(delegate(Type currentType)
                                {
                                    return type == currentType;
                                });
        }
    }
}
