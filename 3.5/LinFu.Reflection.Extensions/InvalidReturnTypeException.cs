using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public class InvalidReturnTypeException : Exception
    {
        private Type _expectedReturnType;
        private Type _actualType;
        private string _methodName;
        public InvalidReturnTypeException(string methodName, Type expectedReturnType, Type actualType)
        {
            _methodName = methodName;
            _expectedReturnType = expectedReturnType;
            _actualType = actualType;
        }
        public override string Message
        {
            get
            {
                string message = string.Format("The return type for method '{0}' should be compatible with type '{1}'. Its actual return type is '{2}'",
                    _methodName,
                    _expectedReturnType.FullName,
                    _actualType.FullName);

                return message;
            }
        }
    }
}
