using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a type that converts functors into method call filter instances.
    /// </summary>
    public class MethodCallFilterAdapter : IMethodCallFilter
    {
        private readonly Func<MethodReference, bool> _hostMethodFilter;
        private readonly Func<MethodReference, bool> _methodCallFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallFilterAdapter"/> class.
        /// </summary>
        /// <param name="hostMethodFilter">The method filter that will determine the host methods that will be modified for interception.</param>
        /// <param name="methodCallFilter">The method filter that will determine which method calls will be intercepted.</param>
        public MethodCallFilterAdapter(Func<MethodReference, bool> hostMethodFilter, Func<MethodReference, bool> methodCallFilter)
        {
            _hostMethodFilter = hostMethodFilter;
            _methodCallFilter = methodCallFilter;
        }

        /// <summary>
        /// Determines whether or not a particular method call should be intercepted.
        /// </summary>
        /// <param name="targetType">The host type that contains the method call.</param>
        /// <param name="hostMethod">The method that contains the current method call.</param>
        /// <param name="currentMethodCall">The method call to be intercepted.</param>
        /// <returns>Returns <c>true</c> if the method call should be intercepted; otherwise, it will return <c>false</c>.</returns>
        public bool ShouldWeave(TypeReference targetType, MethodReference hostMethod, MethodReference currentMethodCall)
        {
            return _hostMethodFilter(hostMethod) && _methodCallFilter(currentMethodCall);
        }        
    }
}
