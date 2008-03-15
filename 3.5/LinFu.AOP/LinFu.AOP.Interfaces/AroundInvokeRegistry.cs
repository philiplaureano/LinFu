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
            List<IAroundInvoke> resultList = new List<IAroundInvoke>();

            foreach (var provider in _providers)
            {
                var aroundInvoke = provider.GetSurroundingImplementation(context);

                if (aroundInvoke == null)
                    continue;

                resultList.Add(aroundInvoke);
            }

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
