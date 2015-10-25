using System;
using System.Collections.Generic;
using System.Linq;

namespace LinFu.AOP.Interfaces
{
    class CompositeAroundInvokeProvider : IAroundInvokeProvider
    {
        /// <summary>
        /// Gets or sets the valud indicating the Predicates that will determine weather or not
        /// the method should be intercepted by the associated <see cref="IAroundInvoke"/>.
        /// </summary>
        public IDictionary<IAroundInvoke, Func<IInvocationInfo, bool>> MethodInvokesAndPredicates { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeAroundInvokeProvider"/> class.
        /// </summary>
        /// <param name="aroundInvokeAndPredicates">
        /// Dictionary of <see cref="IAroundInvoke"/> and predicates to determine if a method should be intercepted.
        /// </param>
        /// <remarks>
        /// Specifing null for the aroundInvokeAndPredicates predicate will use the default ShouldReplace predicate
        /// in <see cref="CompositeAroundInvokeProvider"/>.
        /// </remarks>
        public CompositeAroundInvokeProvider(IDictionary<IAroundInvoke, Func<IInvocationInfo, bool>> aroundInvokeAndPredicates)
        {
            if (aroundInvokeAndPredicates != null)
            {
                MethodInvokesAndPredicates = new Dictionary<IAroundInvoke, Func<IInvocationInfo, bool>>();
                foreach (var invokeAndPredicate in aroundInvokeAndPredicates)
                {
                    MethodInvokesAndPredicates.Add(invokeAndPredicate.Key, invokeAndPredicate.Value ?? ShouldReplace);
                }
            }
        }

        /// <summary>
        /// Determines whether or not a particular method body should be invoked around at runtime.
        /// </summary>
        /// <param name="context">The context surrounding the method call.</param>
        /// <returns>Returns <c>true</c>.</returns>
        public virtual bool ShouldReplace(IInvocationInfo context)
        {
            return true;
        }

        #region IAroundInvokeProvider Members
        /// <summary>
        /// Gets the surrounding implemetation for a given <see cref="IInvocationInfo">invocation context</see>.
        /// </summary>
        /// <param name="context">The context surrounding the method call.</param>
        /// <returns>The interceptor that will invoke around the method at runtime.</returns>
        public IAroundInvoke GetSurroundingImplementation(IInvocationInfo context)
        {
            var invokes = new HashSet<IAroundInvoke>();
            foreach (var invokePredicate in MethodInvokesAndPredicates.Where(x => x.Value != null && x.Value(context)))
            {
                invokes.Add(invokePredicate.Key);
            }
            return (invokes.Count > 0) ? new CompositeAroundInvoke(invokes) : null;
        }
        #endregion
    }
}
