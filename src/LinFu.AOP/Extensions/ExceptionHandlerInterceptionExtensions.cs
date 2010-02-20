using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    public static class ExceptionHandlerInterceptionExtensions
    {
        public static void InterceptAllExceptions(this IReflectionVisitable visitable)
        {
            var filter = GetMethodFilter();
            InterceptExceptions(visitable, filter);
        }
       

        public static void InterceptAllExceptions(this IReflectionStructureVisitable visitable)
        {
            var filter = GetMethodFilter();
            InterceptExceptions(visitable, filter);
        }

        public static void InterceptExceptions(this IReflectionStructureVisitable visitable, Func<MethodReference, bool> methodFilter)
        {
            if (visitable == null)
                throw new ArgumentNullException("visitable");

            var catchAllThrownExceptions = new CatchAllThrownExceptions();
            visitable.WeaveWith(catchAllThrownExceptions, methodFilter);
        }

        public static void InterceptExceptions(this IReflectionVisitable visitable, Func<MethodReference, bool> methodFilter)
        {
            if (visitable == null)
                throw new ArgumentNullException("visitable");

            var catchAllThrownExceptions = new CatchAllThrownExceptions();
            visitable.WeaveWith(catchAllThrownExceptions, methodFilter);
        }

        private static Func<MethodReference, bool> GetMethodFilter()
        {
            return method =>
            {
                var actualMethod = method.Resolve();
                return actualMethod.HasBody;
            };
        }
    }
}
