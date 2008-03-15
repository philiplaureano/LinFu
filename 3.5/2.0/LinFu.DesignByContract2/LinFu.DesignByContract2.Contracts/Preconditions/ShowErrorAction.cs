using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public class ShowErrorAction
    {
        private AdHocPrecondition<object> _precondition;
        public ShowErrorAction(AdHocPrecondition<object> precondition, IMethodContract contract)
        {
            _precondition = precondition;

            contract.Preconditions.Add(_precondition);
        }

        public void OtherwisePrint(string text)
        {
            _precondition.ShowErrorHandler = delegate(TextWriter writer, object target, InvocationInfo info)
                                                 {
                                                     writer.WriteLine(text);
                                                 };
        }
    }
}
