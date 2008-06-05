using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public static class AroundInvokeRegistry
    {
        private static readonly List<IAroundInvokeProvider> _providers = new List<IAroundInvokeProvider>();
        public static IAroundInvoke GetSurroundingImplementation(IInvocationContext context)
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
        public static List<IAroundInvokeProvider> Providers
        {
            get { return _providers; }
        }
    }
}
