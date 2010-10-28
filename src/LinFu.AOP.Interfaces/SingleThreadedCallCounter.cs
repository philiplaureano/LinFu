using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    internal class SingleThreadedCallCounter : ICallCounter
    {
        private readonly Dictionary<object, Counter<MethodBase>> _counts = new Dictionary<object, Counter<MethodBase>>();
        private readonly object _lock = new object();
        public void Increment(IInvocationInfo context)
        {
            var instance = context.Target;
            var targetMethod = context.TargetMethod;
            lock (_lock)
            {
                if (!_counts.ContainsKey(instance))
                    _counts[instance] = new Counter<MethodBase>();

                _counts[instance].Increment();
            }
        }

        public void Decrement(IInvocationInfo context)
        {
            var instance = context.Target;
            var targetMethod = context.TargetMethod;

            if (!_counts.ContainsKey(instance))
                return;

            lock (_lock)
            {
                var counter = _counts[instance];
                counter.Decrement();

                // Remove the counter once it hits zero
                var modifiedCount = counter.GetCount();
                if (modifiedCount <= 0)
                    _counts.Remove(instance);
            }
        }

        public int GetPendingCalls(IInvocationInfo context)
        {
            var result = 0;
            var instance = context.Target;
            var targetMethod = context.TargetMethod;

            if (!_counts.ContainsKey(instance))
                return 0;

            lock (_lock)
            {
                result = _counts[instance].GetCount();
            }


            return result;
        }
    }
}
