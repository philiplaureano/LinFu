using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    public static class ExceptionHandlerInterceptionExtensions
    {
        public static void InterceptAllExceptions(IReflectionVisitable visitable)
        {
            Func<MethodReference, bool> filter = method =>
                                                     {
                                                         var actualMethod = method.Resolve();
                                                         return actualMethod.HasBody;
                                                     };
        }

        public static void InterceptAllExceptions(IReflectionStructureVisitable visitable)
        {            

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
    }
}
