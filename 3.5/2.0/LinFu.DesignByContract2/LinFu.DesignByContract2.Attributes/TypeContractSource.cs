using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.DesignByContract2.Attributes
{
    public class TypeContractSource : IContractSource
    {
        private readonly Type _sourceType;
        public TypeContractSource(Type sourceType)
        {
            _sourceType = sourceType;
        }
        #region IContractSource Members

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _sourceType.GetCustomAttributes(attributeType, inherit);
        }

        public MethodInfo[] GetMethods()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return _sourceType.GetMethods(flags);
        }

        #endregion
    }
}
