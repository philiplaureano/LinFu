using System;
using System.Reflection;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public abstract class DesignByContractException : Exception
    {
        private InvocationInfo _info;

        protected DesignByContractException(string message, InvocationInfo info)
            : base(FormatMessage(message, info))
        {
            _info = info;
        }

        public InvocationInfo InvocationInfo
        {
            get { return _info; }
        }

        public override string StackTrace
        {
            get
            {
                if (_info != null)  
                    return _info.StackTrace.ToString();

                return base.StackTrace;
            }
        }

        private static string FormatMessage(string message, InvocationInfo info)
        {
            if (info == null)
                return message;

            Type declaringType = info.TargetMethod.DeclaringType;
            MethodInfo targetMethod = info.TargetMethod;
            string fullName = string.Format("{0}.{1}()", declaringType.Name, targetMethod.Name);

            string result = string.Format("{0}: {1}", fullName, message);

            return result;
        }
    }
}

