namespace LinFu.AOP.Interfaces
{
    /// <summary>
    ///     Represents the boilerplate implementation for a <see cref="IMethodReplacementProvider" /> instance.
    /// </summary>
    public abstract class BaseMethodReplacementProvider : IMethodReplacementProvider, IAroundInvoke
    {
        private readonly ICallCounter _counter = new MultiThreadedCallCounter();

        public void AfterInvoke(IInvocationInfo context, object returnValue)
        {
            _counter.Decrement(context);
        }

        public void BeforeInvoke(IInvocationInfo context)
        {
            _counter.Increment(context);
        }

        /// <summary>
        ///     Determines whether or not the current method implementation can be replaced.
        /// </summary>
        /// <param name="host">The target instance of the method call.</param>
        /// <param name="context">The <see cref="IInvocationInfo" /> that describes the context of the method call.</param>
        /// <returns><c>true</c> if the method can be intercepted; otherwise, it will return <c>false</c>.</returns>
        public bool CanReplace(object host, IInvocationInfo context)
        {
            var pendingCalls = _counter.GetPendingCalls(context);

            if (pendingCalls > 0)
                return false;

            return ShouldReplace(host, context);
        }

        /// <summary>
        ///     Obtains the <see cref="IInterceptor" /> instance that will be used to replace the current method call.
        /// </summary>
        /// <param name="host">The target instance of the method call.</param>
        /// <param name="context">The <see cref="IInvocationInfo" /> that describes the context of the method call.</param>
        /// <returns>The interceptor that will intercept the method call itself.</returns>
        public IInterceptor GetMethodReplacement(object host, IInvocationInfo context)
        {
            var pendingCalls = _counter.GetPendingCalls(context);

            if (pendingCalls > 0)
                return null;

            var methodReplacement = GetReplacement(host, context);
            return new CountingInterceptor(_counter, methodReplacement);
        }

        protected virtual bool ShouldReplace(object host, IInvocationInfo context)
        {
            return _counter.GetPendingCalls(context) == 0;
        }

        /// <summary>
        ///     Obtains the <see cref="IInterceptor" /> instance that will be used to replace the current method call.
        /// </summary>
        /// <param name="host">The target instance of the method call.</param>
        /// <param name="context">The <see cref="IInvocationInfo" /> that describes the context of the method call.</param>
        /// <returns>The interceptor that will intercept the method call itself.</returns>
        protected abstract IInterceptor GetReplacement(object host, IInvocationInfo context);
    }
}