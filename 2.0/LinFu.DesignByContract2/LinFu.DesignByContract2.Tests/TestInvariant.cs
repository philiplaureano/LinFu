using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;
namespace LinFu.DesignByContract2.Tests
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TestInvariantAttribute : Attribute, IInvariant
    {
        #region IInvariant Members

        public bool Check(object target, InvocationInfo info, InvariantState callState)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ShowError(TextWriter output, object target, LinFu.DynamicProxy.InvocationInfo info, InvariantState callState)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool AppliesTo(object target, InvocationInfo info)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Catch(Exception ex)
        {

        }
        #endregion
    }
}
