using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts
{
    public class AdHocContract : IMethodContract, IContractProvider, ITypeContract
    {
        private IList<IInvariant> _invariants = new List<IInvariant>();
        private IList<IPostcondition> _postconditions = new List<IPostcondition>();
        private IList<IPrecondition> _preconditions = new List<IPrecondition>();

        public IList<IPrecondition> Preconditions
        {
            get { return _preconditions; }
        }
        public IList<IPostcondition> Postconditions
        {
            get { return _postconditions; }
        }
        public ITypeContract GetTypeContract(Type targetType)
        {
            TypeContract contract = new TypeContract();

            foreach (IInvariant invariant in _invariants)
            {
                _invariants.Add(invariant);
            }

            return contract;
        }

        public IMethodContract GetMethodContract(Type targetType, InvocationInfo info)
        {
            MethodContract contract = new MethodContract();
            foreach (IPrecondition precondition in _preconditions)
            {
                contract.Preconditions.Add(precondition);
            }

            foreach (IPostcondition postcondition in _postconditions)
            {
                contract.Postconditions.Add(postcondition);
            }

            return contract;
        }

        public IList<IInvariant> Invariants
        {
            get { return _invariants; }
        }
    }
}
