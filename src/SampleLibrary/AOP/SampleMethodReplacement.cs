using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleMethodReplacement : IInterceptor
    {
        private bool _called;

        public bool HasBeenCalled
        {
            get { return _called; }
        }

        #region IInterceptor Members

        public object Intercept(IInvocationInfo info)
        {
            _called = true;
            return null;
        }

        #endregion
    }
}