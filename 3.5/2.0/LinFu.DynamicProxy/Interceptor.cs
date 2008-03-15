using System;
using System.Diagnostics;
using System.Reflection;

namespace LinFu.DynamicProxy
{
    public abstract class Interceptor : IInterceptor
    {
        protected Interceptor()
        {
        }

        #region IInterceptor Members

        public abstract object Intercept(InvocationInfo info);

        #endregion

        public static implicit operator InterceptorHandler(Interceptor interceptor)
        {
            return interceptor.Intercept;
        }

        public object Intercept(object proxy, MethodInfo targetMethod,
                                StackTrace trace, Type[] genericTypeArgs,
                                object[] args)
        {
            InvocationInfo info = new InvocationInfo(proxy, targetMethod, trace, genericTypeArgs, args);

            return Intercept(info);
        }
    }
}