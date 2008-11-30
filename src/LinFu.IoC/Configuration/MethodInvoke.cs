using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that invokes methods.
    /// </summary>
    public class MethodInvoke : BaseMethodInvoke<MethodInfo>
    {
        /// <summary>
        /// Initializes the class with the default values.
        /// </summary>
        public MethodInvoke()
        {
            MethodBuilder = new ReflectionMethodBuilder<MethodInfo>();          
        }

        /// <summary>
        /// Invokes the <paramref name="targetMethod"/> with the given <paramref name="arguments"/>.
        /// </summary>
        /// <param name="target">The target instance.</param>
        /// <param name="originalMethod">The original method that describes the target method.</param>
        /// <param name="targetMethod">The actual method that will be invoked.</param>
        /// <param name="arguments">The method arguments.</param>
        /// <returns>The return value from the target method.</returns>
        protected override object DoInvoke(object target, MethodInfo originalMethod, MethodBase targetMethod, object[] arguments)
        {
            var actualArguments = new List<object>();

            // Only instance methods need a target
            if (!originalMethod.IsStatic)
                actualArguments.Add(target);

            actualArguments.AddRange(arguments);
            object result = null;
            try
            {
                result = targetMethod.Invoke(null, actualArguments.ToArray());
            }
            catch(TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            return result;
        }
    }
}
