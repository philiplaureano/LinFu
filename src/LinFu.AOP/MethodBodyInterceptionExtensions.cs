using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an extension class that adds method body interception support to the Mono.Cecil object model.
    /// </summary>
    public static class MethodBodyInterceptionExtensions
    {
        public static void InterceptMethodBody(this IReflectionVisitable target, Func<MethodReference, bool> methodFilter)
        {
            Func<TypeReference, bool> typeFilter = type =>
                                                       {
                                                           var actualType = type.Resolve();
                                                           if (actualType.IsValueType || actualType.IsInterface)
                                                               return false;

                                                           return actualType.IsClass;                                                           
                                                       };

            target.Accept(new ImplementModifiableType(typeFilter));
            
            var interceptMethodBody = new InterceptMethodBody(methodFilter);
            target.WeaveWith(interceptMethodBody, methodFilter);
        }
    }
}
