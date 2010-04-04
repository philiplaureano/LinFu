using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleAroundInvoke : IAroundInvoke
    {
        public bool BeforeInvokeWasCalled
        {
            get; private set;
        }

        public bool AfterInvokeWasCalled
        {
            get; private set;
        }

        public void BeforeInvoke(IInvocationInfo info)
        {
            BeforeInvokeWasCalled = true;
        }

        public void AfterInvoke(IInvocationInfo info, object returnValue)
        {
            AfterInvokeWasCalled = true;
        }
    }
}
