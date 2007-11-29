using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.Common;
using LinFu.Reflection;

namespace LinFu.DesignByContract2.Attributes
{
    public class MethodFinder : IMethodFinder 
    {
        private Dictionary<MethodFinderEntry, MethodInfo> _cache = new Dictionary<MethodFinderEntry, MethodInfo>();
        #region IMethodFinder Members

        public System.Reflection.MethodInfo FindMatchingMethod(IContractSource targetType, System.Reflection.MethodInfo sampleMethod)
        {
            MethodFinderEntry cacheKey = new MethodFinderEntry(targetType, sampleMethod);

            if (_cache.ContainsKey(cacheKey))
                return _cache[cacheKey];
            
            PredicateBuilder builder = new PredicateBuilder();
            builder.ReturnType = sampleMethod.ReturnType;
            builder.IsPublic = sampleMethod.IsPublic;
            builder.IsProtected = sampleMethod.IsFamily;
            builder.MethodName = sampleMethod.Name;
            builder.ParameterTypes.AddRange(sampleMethod.GetParameters());
            
            
            if (sampleMethod.IsGenericMethod)
                builder.TypeArguments.AddRange(sampleMethod.GetGenericArguments());

            Predicate<MethodInfo> finderPredicate = builder.CreatePredicate();

            // Search for methods declared only on this type
            
            MethodInfo[] methods = targetType.GetMethods();

            FuzzyFinder<MethodInfo> finder = new FuzzyFinder<MethodInfo>();
            finder.Tolerance = .60;
            MethodInfo result = finder.Find(finderPredicate, methods);

            if (result != null)
                _cache[cacheKey] = result;
            
            return result;
        }

        #endregion
    }
}
