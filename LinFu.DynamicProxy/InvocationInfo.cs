using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace LinFu.DynamicProxy
{
    public class InvocationInfo
    {
        private object[] _args;
        private object _proxy;
        private MethodInfo _targetMethod;
        private StackTrace _trace;
        private Type[] _typeArgs;

        public InvocationInfo(object proxy, MethodInfo targetMethod,
                              StackTrace trace, Type[] genericTypeArgs, object[] args)
        {
            _proxy = proxy;
            _targetMethod = targetMethod;
            _typeArgs = genericTypeArgs;
            _args = args;
            _trace = trace;
        }

        public object Target
        {
            get { return _proxy; }
        }

        public MethodInfo TargetMethod
        {
            get { return _targetMethod; }
        }

        public StackTrace StackTrace
        {
            get { return _trace; }
        }

        public MethodInfo CallingMethod
        {
            get { return (MethodInfo) _trace.GetFrame(0).GetMethod(); }
        }

        public Type[] TypeArguments
        {
            get { return _typeArgs; }
        }

        public object[] Arguments
        {
            get { return _args; }
        }

        public void SetArgument(int position, object arg)
        {
            _args[position] = arg;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Calling Method: {0,30:G}\n", GetMethodName(CallingMethod));
            builder.AppendFormat("Target Method:{0,30:G}\n", GetMethodName(_targetMethod));
            builder.AppendLine("Arguments:");

            foreach (ParameterInfo info in _targetMethod.GetParameters())
            {
                object currentArgument = _args[info.Position];
                if (currentArgument == null)
                    currentArgument = "(null)";
                builder.AppendFormat("\t{0,10:G}: {1}\n", info.Name, currentArgument.ToString());
            }
            builder.AppendLine();

            return builder.ToString();
        }

        private string GetMethodName(MethodInfo method)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}.{1}", method.DeclaringType.Name, method.Name);
            builder.Append("(");

            ParameterInfo[] parameters = method.GetParameters();
            int parameterCount = parameters != null ? parameters.Length : 0;

            int index = 0;
            foreach (ParameterInfo param in parameters)
            {
                index++;
                builder.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);

                if (index < parameterCount)
                    builder.Append(", ");
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}