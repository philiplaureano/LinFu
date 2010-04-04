using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleAroundInvokeProvider : IAroundInvokeProvider
    {
        private readonly IAroundInvoke _aroundInvoke;
        public SampleAroundInvokeProvider(IAroundInvoke aroundInvoke)
        {
            _aroundInvoke = aroundInvoke;
        }

        public IAroundInvoke GetSurroundingImplementation(IInvocationInfo context)
        {
            return _aroundInvoke;
        }
    }
}
