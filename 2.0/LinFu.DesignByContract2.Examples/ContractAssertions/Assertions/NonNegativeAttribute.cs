using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Attributes;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace CSContractAssertions
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class NonNegativeAttribute : Attribute, IParameterPrecondition, IPostcondition 
    {
        #region IParameterPrecondition Members

        public bool Check(InvocationInfo info, ParameterInfo parameter, object argValue)
        {
            int value = Convert.ToInt32(argValue);
            return value >= 0;
        }

        public void ShowErrorMessage(TextWriter stdOut, InvocationInfo info, 
            ParameterInfo parameter, object argValue)
        {
            stdOut.WriteLine("The parameter '{0}' must be non-negative", parameter.Name);
        }

        #endregion

        #region IContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            if (target == null)
                return false;

            return true;           
        }

        public void Catch(Exception ex)
        {
            // Ignore the error
        }

        #endregion

        #region IPostcondition Members

        public void BeforeMethodCall(object target, InvocationInfo info)
        {
        }

        public bool Check(object target, InvocationInfo info, object returnValue)
        {
            int value = Convert.ToInt32(returnValue);
            return value >= 0;
        }

        public void ShowError(TextWriter output, object target, InvocationInfo info, object returnValue)
        {
            output.WriteLine("The return value must be non-negative");
        }

        #endregion
    }
}
