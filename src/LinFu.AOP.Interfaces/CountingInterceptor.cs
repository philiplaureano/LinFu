using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    internal class CountingInterceptor : IInterceptor
    {
        private readonly ICallCounter _callCounter;
        private readonly IInterceptor _methodReplacement;

        internal CountingInterceptor(ICallCounter callCounter, IInterceptor methodReplacement)
        {
            _callCounter = callCounter;
            _methodReplacement = methodReplacement;
        }

        public object Intercept(IInvocationInfo info)
        {
            _callCounter.Increment(info);

            var returnValue = _methodReplacement.Intercept(info);

            _callCounter.Decrement(info);

            return returnValue;
        }
    }    
}
