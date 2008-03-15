using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Delegates
{
    public class CustomDelegateRedirector : Interceptor
    {
        private readonly CustomDelegate _target;
        public CustomDelegateRedirector(CustomDelegate target)
        {
            _target = target;
        }
        public override object Intercept(InvocationInfo info)
        {
            return _target(info.Arguments);
        }
    }
}
