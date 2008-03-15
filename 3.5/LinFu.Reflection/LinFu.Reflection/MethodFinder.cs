using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.Common;

namespace LinFu.Reflection
{
    internal class MethodFinder : IMethodFinder
    {
        private readonly List<MethodInfo> _cachedResults = new List<MethodInfo>();
        private readonly Dictionary<Type, IEnumerable<MethodInfo>> _methodCache = new Dictionary<Type, IEnumerable<MethodInfo>>();
        #region IMethodFinder Members

        public MethodInfo Find(string methodName, Type targetType, object[] args)
        {
            PredicateBuilder builder = new PredicateBuilder();
            builder.MethodName = methodName;
            builder.MatchParameters = true;
            builder.MatchCovariantParameterTypes = true;

            // Find the method that has a compatible signature
            // and a matching method name
            List<object> arguments = new List<object>();
            if (args != null && args.Length > 0)
                arguments.AddRange(args);
            builder.RuntimeArguments.AddRange(arguments);
            builder.MatchRuntimeArguments = true;

            Predicate<MethodInfo> finderPredicate = builder.CreatePredicate();
            FuzzyFinder<MethodInfo> finder = new FuzzyFinder<MethodInfo>();
            finder.Tolerance = .66;

            // Search for any previous matches
            MethodInfo bestMatch = finder.Find(finderPredicate, _cachedResults);

            if (bestMatch == null)
            {
                // If there isn't a match, search the current type
                // for an existing match
                IEnumerable<MethodInfo> methods = GetMethods(targetType);
                bestMatch = finder.Find(finderPredicate, methods);
            }

            return bestMatch;
        }


        #endregion
        private IEnumerable<MethodInfo> GetMethods(Type targetType)
        {
            if (_methodCache.ContainsKey(targetType))
                return _methodCache[targetType];

            _methodCache[targetType] =
                targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            return _methodCache[targetType];
        }
    }
}
