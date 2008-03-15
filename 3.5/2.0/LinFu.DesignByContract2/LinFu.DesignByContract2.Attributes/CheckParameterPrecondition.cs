using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Attributes
{
    internal class CheckParameterPrecondition : IPrecondition
    {
        private IParameterPrecondition _precondition;
        private ParameterInfo _parameter;
        public CheckParameterPrecondition(ParameterInfo parameter, IParameterPrecondition precondition)
        {
            _precondition = precondition;
            _parameter = parameter;
        }
        #region IPrecondition Members

        public bool Check(object target, InvocationInfo info)
        {
            int position = _parameter.Position;
            bool result = _precondition.Check(info, _parameter, info.Arguments[position]);

            return result;
        }

        public void ShowError(TextWriter stdOut, object target, InvocationInfo info)
        {
            int position = _parameter.Position;
            _precondition.ShowErrorMessage(stdOut, info, _parameter,
                info.Arguments[position]);
        }

        #endregion

        #region IMethodContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            ParameterInfo[] parameters = info.TargetMethod.GetParameters();            
            int parameterCount = parameters != null ? parameters.Length : 0;

            int position = _parameter.Position;

            return position < parameterCount;
        }

        public void Catch(Exception ex)
        {
            _precondition.Catch(ex);
        }
        #endregion
    }
}
