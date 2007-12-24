using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.Delegates
{
    class DefaultInvoker : IInvoker
    {
        #region IInvoker Members

        public object Invoke(object target, MethodBase targetMethod, IEnumerable<object> curriedArguments, IEnumerable<object> invokeArguments)
        {
            List<object> combinedArguments = GetCombinedArguments(targetMethod, curriedArguments, invokeArguments);
            

            ParameterInfo[] parameters = targetMethod.GetParameters();
            int parameterCount = parameters == null ? 0 : parameters.Length;

            object[] args = new object[parameterCount];
            for(int i = 0; i < parameterCount; i++)
            {
                args[i] = combinedArguments[i];
            }

            // HACK: Coerce the argument list into an object array if
            // necessary
            bool isObjectArray = parameters != null && parameters.Length == 1 &&
                                  parameters[0].ParameterType == typeof (object[]);

            if (isObjectArray)                
                args = new object[] { combinedArguments.ToArray() };

            object result = null;
            try
            {
                result = targetMethod.Invoke(target, args);                
            }
            catch(TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            return result;
        }

        private static List<object> GetCombinedArguments(MethodBase targetMethod, 
        IEnumerable<object> curriedArguments, IEnumerable<object> invokeArguments)
        {
            ParameterInfo[] parameters = targetMethod.GetParameters();
            int parameterCount = parameters != null ? parameters.Length : 0;


            List<object> combinedArguments = new List<object>();
            for(int i = 0; i < parameterCount; i++)
            {
                combinedArguments.Add(Args.Lambda);
            }

            AssignArguments(curriedArguments, combinedArguments);            
            AssignArguments(invokeArguments, combinedArguments);            

            return combinedArguments;
        }

        #endregion
        private static void AssignArguments(IEnumerable<object> sourceList, IList<object> targetList)
        {
            Queue<object> source = new Queue<object>(sourceList);
            for (int i = 0; i < targetList.Count; i++)
            {
                if (source.Count == 0)
                    break;

                if (targetList[i] != Args.Lambda)
                    continue;

                object argument = source.Dequeue();

                if (argument is IDeferredArgument)
                {
                    IDeferredArgument deferred = (IDeferredArgument)argument;
                    argument = deferred.Evaluate();
                }
                
                targetList[i] = argument;
            }
        }
    }
}
