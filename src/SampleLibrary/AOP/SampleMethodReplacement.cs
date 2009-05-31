using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleMethodReplacement : IInterceptor
    {
        private bool _called;

        public bool HasBeenCalled
        {
            get
            {
                return _called;
            }
        }

        public object Intercept(IInvocationInfo info)
        {
            _called = true;
            return null;
        }
    }
}
