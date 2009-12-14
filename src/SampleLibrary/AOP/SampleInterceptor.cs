using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleInterceptor : IInterceptor
    {
        public bool HasBeenInvoked
        {
            get; set;
        }

        public object Intercept(IInvocationInfo info)
        {
            HasBeenInvoked = true;
            return null;
        }
    }
}
