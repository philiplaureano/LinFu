using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Contracts.Postconditions;
using LinFu.DesignByContract2.Core;

namespace LinFu.DesignByContract2.Contracts
{
    public static class Ensure
    {
        public static SetMethodFilter On(IMethodContract contract)
        {
            return new SetMethodFilter(contract);
        }
    }
}
