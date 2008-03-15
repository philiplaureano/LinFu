using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Injectors
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ContractForAttribute : Attribute
    {
        private Type _targetType;
        public ContractForAttribute(Type targetType)
        {
            _targetType = targetType;
        }

        public Type TargetType
        {
            get { return _targetType; }
        }
    }
}
