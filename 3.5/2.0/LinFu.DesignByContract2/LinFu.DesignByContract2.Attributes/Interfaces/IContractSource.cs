using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.DesignByContract2.Attributes
{
    public interface IContractSource
    {
        object[] GetCustomAttributes(Type attributeType, bool inherit);
        MethodInfo[] GetMethods();
    }
}
