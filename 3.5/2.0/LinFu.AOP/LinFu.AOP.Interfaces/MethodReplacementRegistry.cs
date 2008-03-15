using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public static class MethodReplacementRegistry
    {
        private static readonly List<IMethodReplacementProvider> _providers = new List<IMethodReplacementProvider>();
        public static IMethodReplacementProvider GetProvider(IInvocationContext context)
        {
            var matches = from p in _providers
                          where p.CanReplace(context)
                          select p;

            var resultList = matches.ToList();

            if (resultList.Count == 0)
                return null;

            return resultList.First();
        }
        public static IList<IMethodReplacementProvider> Providers
        {
            get { return _providers; }
        }
    }
}
