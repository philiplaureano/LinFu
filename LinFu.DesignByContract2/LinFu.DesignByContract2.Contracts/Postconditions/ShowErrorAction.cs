using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public class ShowErrorAction
    {
        private AdHocPostcondition<object> _postcondition;
        public ShowErrorAction(AdHocPostcondition<object> postcondition, IMethodContract contract)
        {
            _postcondition = postcondition;

            contract.Postconditions.Add(_postcondition);
        }

        public void OtherwisePrint(string text)
        {
            _postcondition.ShowErrorHandler = delegate(TextWriter writer, object target, InvocationInfo info, object returnValue)
                                                 {
                                                     writer.WriteLine(text);
                                                 };
        }
    }
}
