using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Contracts.Invariants;
using LinFu.DesignByContract2.Core;

namespace LinFu.DesignByContract2.Contracts
{
    public static class Invariant
    {
        public static SetTypeContract On(ITypeContract contract)
        {
            return new SetTypeContract(contract);
        }
    }
}
