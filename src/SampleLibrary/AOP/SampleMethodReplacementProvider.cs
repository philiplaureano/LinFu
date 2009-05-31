using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleMethodReplacementProvider : IMethodReplacementProvider
    {
        private IInterceptor _interceptor;
        public SampleMethodReplacementProvider(IInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public bool CanReplace(object host, IInvocationInfo info)
        {
            return true;
        }

        public IInterceptor GetMethodReplacement(object host, IInvocationInfo info)
        {
            return _interceptor;
        }
    }
}
