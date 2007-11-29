using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Attributes;

namespace LinFu.DesignByContract2.Tests
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TestParameterPreconditionAttribute : Attribute, IParameterPrecondition
    {
        #region IParameterPrecondition Members

        public bool Check(LinFu.DynamicProxy.InvocationInfo info, System.Reflection.ParameterInfo parameter, object argValue)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ShowErrorMessage(System.IO.TextWriter stdOut, LinFu.DynamicProxy.InvocationInfo info, System.Reflection.ParameterInfo parameter, object argValue)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IMethodContractCheck Members

        public bool AppliesTo(object target, LinFu.DynamicProxy.InvocationInfo info)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Catch(Exception ex)
        {

        }
        #endregion
    }
}
