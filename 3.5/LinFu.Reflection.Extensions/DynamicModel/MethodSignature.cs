using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    internal class MethodSignature : IMethodSignature
    {
        private readonly IEnumerable<Type> _parameterTypes;
        private readonly Type _returnType;
        public MethodSignature(IEnumerable<Type> parameterTypes,
            Type returnType)
        {
            _parameterTypes = parameterTypes;
            _returnType = returnType;
        }
        #region IMethodSignature Members

        public IEnumerable<Type> ParameterTypes
        {
            get
            {
                return _parameterTypes;
            }
        }

        public Type ReturnType
        {
            get
            {
                return _returnType;
            }
        }

        #endregion
    }
}
