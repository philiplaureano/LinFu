using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.DesignByContract2.Attributes
{
    internal struct MethodFinderEntry    
    {
        public IContractSource TargetType;
        public MethodInfo TargetMethod;
        public MethodFinderEntry(IContractSource targetType, MethodInfo targetMethod)
        {
            TargetMethod = targetMethod;
            TargetType = targetType;
        }
    }
}
