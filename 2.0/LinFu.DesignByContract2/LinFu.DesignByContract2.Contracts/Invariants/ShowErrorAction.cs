using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Invariants
{
    public class ShowErrorAction
    {
        private AdHocInvariant<object> _invariant;
        private ITypeContract _contract;
        public ShowErrorAction(AdHocInvariant<object> invariant, ITypeContract contract)
        {
            _invariant = invariant;
            _contract = contract;
        }

        public void OtherwisePrint(string text)
        {
            _invariant.ShowErrorHandler = delegate(TextWriter writer, object target, InvocationInfo info, InvariantState callState)
                                                 {
                                                     writer.WriteLine(text);
                                                 };


            _contract.Invariants.Add(_invariant);
        }
    }
}
