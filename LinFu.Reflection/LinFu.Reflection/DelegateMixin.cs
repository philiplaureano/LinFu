using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.Common;

namespace LinFu.Reflection
{
    internal class DelegateMixin : IMethodMissingCallback
    {
        private MulticastDelegate _target;
        private string _methodName;
        public DelegateMixin(string methodname, MulticastDelegate targetDelegate)
        {
            _methodName = methodname;
            _target = targetDelegate;
        }
        #region IMethodMissingCallback Members

        public void MethodMissing(object source, 
            MethodMissingParameters missingParameters)
        {
            PredicateBuilder builder = new PredicateBuilder();

            // The current method name must match the given method name
            if (_methodName != missingParameters.MethodName)
                return;

            if (missingParameters.Arguments != null)
                builder.RuntimeArguments.AddRange(missingParameters.Arguments);

            builder.MatchRuntimeArguments = true;

            Predicate<MethodInfo> finderPredicate = builder.CreatePredicate();
            FuzzyFinder<MethodInfo> finder = new FuzzyFinder<MethodInfo>();
            finder.Tolerance = .60;

            // Match the criteria against the target delegate
            List<MethodInfo> searchList = new List<MethodInfo>(new MethodInfo[] {_target.Method});

            // Determine if the signature is compatible
            MethodInfo match = finder.Find(finderPredicate, searchList);
            if (match == null)
                return;

            // If the signature is compatible, then execute the method
            MethodInfo targetMethod = _target.Method;

            object result = null;
            try
            {
                result = targetMethod.Invoke(_target.Target, missingParameters.Arguments);
                missingParameters.Handled = true;
            }
            catch (TargetInvocationException ex)
            {
                missingParameters.Handled = false;
                throw ex.InnerException;
            }

            
            missingParameters.ReturnValue = result;
        }

        #endregion

        #region IMethodMissingCallback Members


        public bool CanHandle(MethodInfo method)
        {
            Predicate<MethodInfo> predicate = PredicateBuilder.CreatePredicate(method);
            FuzzyFinder<MethodInfo> finder = new FuzzyFinder<MethodInfo>();
            finder.Tolerance = .66;

            MethodInfo[] searchPool = new MethodInfo[] {_target.Method};
            MethodInfo match = finder.Find(predicate, searchPool);

            return match != null;
        }

        #endregion
    }
}
