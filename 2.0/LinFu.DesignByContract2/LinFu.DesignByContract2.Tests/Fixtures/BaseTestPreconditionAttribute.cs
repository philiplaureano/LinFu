using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BaseTestPreconditionAttribute : Attribute, IPrecondition
    {
        #region IPrecondition Members

        public bool Check(object target, LinFu.DynamicProxy.InvocationInfo info)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ShowError(System.IO.TextWriter output, object target, InvocationInfo info)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IMethodContractCheck Members

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
