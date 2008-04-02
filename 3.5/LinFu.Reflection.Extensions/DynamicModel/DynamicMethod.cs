using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.Common;

namespace LinFu.Reflection.Extensions
{
    public class DynamicMethod : IMethodMissingCallback
    {
        public DynamicMethod()
        {
        }
        public DynamicMethod(MethodSpec methodSpec)
        {
            MethodSpec = methodSpec;
        }
        public MethodSpec MethodSpec { get; set; }
        #region IMethodMissingCallback Members

        public bool CanHandle(MethodInfo method)
        {
            if (MethodSpec == null || MethodSpec.MethodBody == null)
                return false;

            PredicateBuilder builder = new PredicateBuilder();
            builder.MatchCovariantParameterTypes = false;
            builder.MatchCovariantReturnType = false;
            builder.MatchParameterTypes = true;

            builder.ArgumentTypes.AddRange(MethodSpec.ParameterTypes);
            builder.ReturnType = MethodSpec.ReturnType;

            // Find the method that matches the given name or
            // the alias
            var methodNames = new List<string>();
            methodNames.Add(MethodSpec.MethodName);

            if (MethodSpec.Aliases.Count > 0)
                methodNames.AddRange(MethodSpec.Aliases);

            FuzzyFinder<MethodInfo> finder = new FuzzyFinder<MethodInfo>();
            finder.Tolerance = .66;

            bool found = false;
            var itemList = new MethodInfo[] { method };
            foreach (var name in methodNames)
            {
                builder.MethodName = name;
                var predicate = builder.CreatePredicate();

                MethodInfo match = finder.Find(predicate, itemList);
                if (match == null)
                    continue;

                found = true;
                break;
            }

            return found;
        }

        public void MethodMissing(object source, MethodMissingParameters missingParameters)
        {
            if (MethodSpec == null)
                return;

            string currentMethodName = missingParameters.MethodName;
            // Make sure that the method name matches either the
            // actual method name or a name from the list of method aliases
            List<string> names = new List<string>();
            names.Add(MethodSpec.MethodName);
            names.AddRange(MethodSpec.Aliases);

            bool isMatch = names.Contains(currentMethodName);
            if (!isMatch)
                return;

            // Match the parameter count
            int parameterCount = missingParameters.Arguments == null ? 0 : missingParameters.Arguments.Length;

            if (parameterCount != MethodSpec.ParameterTypes.Count)
                return;

            // Match the parameter types
            bool isCompatible = true;
            int index = 0;
            foreach (Type parameterType in MethodSpec.ParameterTypes)
            {
                Type argumentType = null;
                object currentArgument = missingParameters.Arguments[index++];

                if (currentArgument != null)
                    argumentType = currentArgument.GetType();

                if (argumentType == null)
                    continue;

                // Match the parameter type with the argument type
                // Note: Covariant typing is enabled by default
                if (parameterType.IsAssignableFrom(argumentType))
                    continue;

                isCompatible = false;
                break;
            }

            if (!isCompatible)
                return;

            InvokeMethodImplementation(source, missingParameters, currentMethodName);
        }

        private void InvokeMethodImplementation(object source, MethodMissingParameters missingParameters, string currentMethodName)
        {
            missingParameters.Handled = true;

            var body = MethodSpec.MethodBody;

            if (body == null)
                throw new NotImplementedException(string.Format("The method '{0}' does not have an implementation", currentMethodName));

            var signature = new MethodSignature(MethodSpec.ParameterTypes, MethodSpec.ReturnType);

            missingParameters.ReturnValue = body.Invoke(source, MethodSpec.MethodName,
                signature, missingParameters.Arguments);

            if (missingParameters.ReturnValue == null || MethodSpec.ReturnType == typeof(void))
                return;

            // Verify that the return value is compatible with the return type
            Type actualReturnType = missingParameters.ReturnValue.GetType();

            if (!MethodSpec.ReturnType.IsAssignableFrom(actualReturnType))
                throw new InvalidReturnTypeException(MethodSpec.MethodName, MethodSpec.ReturnType, actualReturnType);
        }

        #endregion
    }
}
