using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace LinFu.AOP.Interfaces
{
    public class InvocationContext : IInvocationContext
    {
        private object[] _args;
        private object _proxy;
        private MethodInfo _targetMethod;
        private StackTrace _trace;
        private Type[] _typeArgs;
        private Type[] _parameterTypes;
        private Type _returnType;
        public InvocationContext(object proxy, MethodInfo targetMethod, StackTrace trace, 
            Type[] genericTypeArgs, Type[] parameterTypes, Type returnType, object[] args)
        {
            _proxy = proxy;
            _targetMethod = targetMethod;
            _typeArgs = genericTypeArgs;
            _args = args;
            _trace = trace;
            _parameterTypes = parameterTypes;
            _returnType = returnType;
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
            get { return (MethodInfo)_trace.GetFrame(0).GetMethod(); }
        }

        public Type[] TypeArguments
        {
            get { return _typeArgs; }
        }

        public object[] Arguments
        {
            get { return _args; }
        }
        public Type[] ParameterTypes
        {
            get { return _parameterTypes; }
        }
        public Type ReturnType
        {
            get { return _returnType; }
        }
        public void SetArgument(int position, object arg)
        {
            _args[position] = arg;
        }
    }
}
