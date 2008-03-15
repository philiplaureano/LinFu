using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public class ShowParameterErrorAction
    {
        private IMethodContract _contract;
        private AdHocPrecondition<object> _precondition;

        public ShowParameterErrorAction(IMethodContract contract, AdHocPrecondition<object> precondition)
        {
            _contract = contract;
            _precondition = precondition;
        }

        public void OtherwisePrint(string message)
        {
            _precondition.ShowErrorHandler = delegate(TextWriter writer, object target, InvocationInfo info)
                                                 {
                                                     writer.WriteLine(message);
                                                 };

            _contract.Preconditions.Add(_precondition);
        }
    }
}
