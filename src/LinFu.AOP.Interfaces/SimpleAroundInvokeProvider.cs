using System;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents the simplest possible <see cref="IAroundInvokeProvider"/> implementation
    /// that will allow the user to use the original around invoke implementation as part
    /// of the interceptor call.
    /// </summary>
    public class SimpleAroundInvokeProvider : IAroundInvokeProvider
    {
        /// <summary>
        /// Gets or sets the value indicating the Predicate that will determine whether or not 
        /// the method should be intercepted.
        /// </summary>
        /// <value>The interceptor predicate.</value>
        public Func<IInvocationInfo, bool> MethodInvokePredicate { get; set; }

        private readonly IAroundInvoke _around;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleAroundInvokeProvider"/> class.
        /// </summary>
        /// <param name="around">The around invoke interceptor.</param>
        public SimpleAroundInvokeProvider(IAroundInvoke around)
        {
            _around = around;
            MethodInvokePredicate = ShouldReplace;
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
            if (MethodInvokePredicate == null || !MethodInvokePredicate(context))
                return null;
            return _around;
        }
        #endregion
    }
}
