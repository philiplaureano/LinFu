using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Delegates
{
    public class InterfaceMethodInfo
    {
        private string _methodName;
        private Type _returnType;
        private Type[] _argumentTypes;

        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; }
        }

        public Type ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }

        public Type[] ArgumentTypes
        {
            get { return _argumentTypes; }
            set { _argumentTypes = value; }
        }
    }
}
