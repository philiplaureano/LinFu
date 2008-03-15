    using System;
using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

namespace LinFu.DesignByContract2.Attributes
{
    public interface IMethodFinder
    {
        MethodInfo FindMatchingMethod(IContractSource targetType, MethodInfo sampleMethod);
    }
}
