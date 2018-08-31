using System;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    ///     Represents the simplest possible <see cref="IMethodReplacementProvider" /> implementation
    ///     that will allow the user to use the original method body implementation as part
    ///     of the interceptor call.
    /// </summary>
    public class SimpleMethodReplacementProvider : BaseMethodReplacementProvider
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleMethodReplacementProvider" /> class.
        /// </summary>
        /// <param name="replacement">The method body replacement interceptor.</param>
        public SimpleMethodReplacementProvider(IInterceptor replacement)
        {
            MethodReplacement = replacement;
        }

        /// <summary>
        ///     Gets or sets the value indicating the Predicate that will determine whether or not
        ///     the method should be intercepted.
        /// </summary>
        /// <value>The interceptor predicate.</value>
        public Func<IInvocationInfo, bool> MethodReplacementPredicate { get; set; }

        /// <summary>
        ///     Gets or sets the value indicating the actual <see cref="IInterceptor" />
        ///     instance that will provide the method body implementations.
        /// </summary>
        /// <value>The interceptor that will swap the method bodies at runtime.</value>
        public IInterceptor MethodReplacement { get; set; }

        /// <summary>
        ///     Determines whether or not a particular method body should be replaced at runtime.
        /// </summary>
        /// <param name="host">The host instance that contains the target method.</param>
        /// <param name="context">The context surrounding the method call.</param>
        /// <returns>Returns <c>true</c> if the method body should be swapped; otherwise, it will return <c>false</c>.</returns>
        protected override bool ShouldReplace(object host, IInvocationInfo context)
        {
            if (MethodReplacementPredicate == null)
                return true;

            return MethodReplacementPredicate(context);
        }

        /// <summary>
        ///     Gets the method replacement for a given <see cref="IInvocationInfo">invocation context</see>.
        /// </summary>
        /// <param name="host">The host instance that contains the target method.</param>
        /// <param name="context">The context surrounding the method call.</param>
        /// <returns>The interceptor that will swap the method bodies at runtime.</returns>
        protected override IInterceptor GetReplacement(object host, IInvocationInfo context)
        {
            return MethodReplacement;
        }
    }
}