using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public static class AroundMethodBodyRegistry
    {
        private static readonly List<IAroundInvokeProvider> _providers = new List<IAroundInvokeProvider>();
        private static readonly object _lock = new object();

        public static IAroundInvoke GetSurroundingImplementation(IInvocationInfo context)
        {
            var resultList = (from p in _providers
                              where p != null
                              let aroundInvoke = p.GetSurroundingImplementation(context)
                              where aroundInvoke != null
                              select aroundInvoke).ToList();

            if (resultList.Count == 0)
                return null;

            return new CompositeAroundInvoke(resultList);
        }

        public static void AddProvider(IAroundInvokeProvider provider)
        {
            lock (_lock)
            {
                _providers.Add(provider);
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _providers.Clear();
            }
        }
    }
}
