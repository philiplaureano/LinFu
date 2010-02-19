using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    /// Represents an extension class that adds method body interception support to the Mono.Cecil object model.
    /// </summary>
    public static class MethodBodyInterceptionExtensions
    {
        public static void InterceptAllMethodBodies(this IReflectionStructureVisitable target)
        {
            target.InterceptMethodBody(m => true);
        }

        public static void InterceptAllMethodBodies(this IReflectionVisitable target)
        {
            target.InterceptMethodBody(m => true);
        }

        public static void InterceptMethodBody(this IReflectionStructureVisitable target, Func<MethodReference, bool> methodFilter)
        {
            var typeFilter = GetTypeFilter();
            target.Accept(new ImplementModifiableType(typeFilter));

            var interceptMethodBody = new InterceptMethodBody(methodFilter);
            target.WeaveWith(interceptMethodBody, methodFilter);
        }        

        public static void InterceptMethodBody(this IReflectionVisitable target, Func<MethodReference, bool> methodFilter)
        {
            var typeFilter = GetTypeFilter();
            target.Accept(new ImplementModifiableType(typeFilter));
            
            var interceptMethodBody = new InterceptMethodBody(methodFilter);
            target.WeaveWith(interceptMethodBody, methodFilter);
        }

        private static Func<TypeReference, bool> GetTypeFilter()
        {
            return type =>
                       {
                           var actualType = type.Resolve();
                           if (actualType.IsValueType || actualType.IsInterface)
                               return false;

                           return actualType.IsClass;
                       };
        }
    }
}