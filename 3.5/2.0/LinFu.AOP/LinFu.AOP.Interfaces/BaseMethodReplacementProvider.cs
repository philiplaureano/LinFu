using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public abstract class BaseMethodReplacementProvider : IMethodReplacementProvider, IAroundInvoke
    {        
        private ICallCounter _counter = new MultiThreadedCallCounter();
        protected BaseMethodReplacementProvider()
        {
        }

        #region IMethodReplacementProvider Members

        public bool CanReplace(IInvocationContext context)
        {
            int pendingCalls = _counter.GetPendingCalls(context);

            if (pendingCalls > 0)
                return false;

            return ShouldReplace(context);
        }

        public IMethodReplacement GetMethodReplacement(IInvocationContext context)
        {
            int pendingCalls = _counter.GetPendingCalls(context);

            if (pendingCalls > 0)
                return null;

            return GetReplacement(context);
        }

        #endregion

        #region IAroundInvoke Members

        public void AfterInvoke(IInvocationContext context, object returnValue)
        {
            _counter.Decrement(context);
        }

        public void BeforeInvoke(IInvocationContext context)
        {
            _counter.Increment(context);
        }

        #endregion
        protected virtual bool ShouldReplace(IInvocationContext context)
        {
            return true;
        }
        protected abstract IMethodReplacement GetReplacement(IInvocationContext context);
    }
}
