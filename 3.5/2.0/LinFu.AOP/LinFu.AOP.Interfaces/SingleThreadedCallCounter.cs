using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.AOP.Interfaces
{
    public class SingleThreadedCallCounter : ICallCounter
    {
        private Dictionary<object, Counter<MethodInfo>> _counts = new Dictionary<object, Counter<MethodInfo>>();
        #region ICallCounter Members

        public void Increment(IInvocationContext context)
        {
            object instance = context.Target;
            MethodInfo targetMethod = context.TargetMethod;
            lock (_counts)
            {
                if (!_counts.ContainsKey(instance))
                    _counts[instance] = new Counter<MethodInfo>();

                _counts[instance].Increment();
            }
        }

        public void Decrement(IInvocationContext context)
        {
            object instance = context.Target;
            MethodInfo targetMethod = context.TargetMethod;

            if (!_counts.ContainsKey(instance))
                return;

            lock (_counts)
            {
                var counter = _counts[instance];                
                counter.Decrement();
                
                // Remove the counter once it hits zero
                int modifiedCount = counter.GetCount();
                if (modifiedCount <= 0)
                    _counts.Remove(instance);
            }
        }

        public int GetPendingCalls(IInvocationContext context)
        {
            int result = 0;
            object instance = context.Target;
            MethodInfo targetMethod = context.TargetMethod;

            if (!_counts.ContainsKey(instance))
                return 0;

            result = _counts[instance].GetCount();

            return result;
        }

        #endregion
    }
}
