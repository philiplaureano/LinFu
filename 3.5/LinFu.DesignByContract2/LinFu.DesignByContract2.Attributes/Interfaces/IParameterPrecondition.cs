using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Attributes
{
    public interface IParameterPrecondition : IMethodContractCheck
    {
        bool Check(InvocationInfo info, ParameterInfo parameter, object argValue);
        void ShowErrorMessage(TextWriter stdOut, InvocationInfo info, ParameterInfo parameter,
            object argValue);
    }
}
