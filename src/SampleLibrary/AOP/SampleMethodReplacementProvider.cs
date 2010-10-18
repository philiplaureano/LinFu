using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleMethodReplacementProvider : IMethodReplacementProvider
    {
        private readonly IInterceptor _interceptor;

        public SampleMethodReplacementProvider(IInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        #region IMethodReplacementProvider Members

        public bool CanReplace(object host, IInvocationInfo info)
        {
            return true;
        }

        public IInterceptor GetMethodReplacement(object host, IInvocationInfo info)
        {
            return _interceptor;
        }

        #endregion
    }
}