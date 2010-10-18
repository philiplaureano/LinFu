using System;
using LinFu.AOP.Interfaces;

namespace LinFu.UnitTests.Proxy
{
    public class MockInterceptor : IInterceptor
    {
        private readonly Func<IInvocationInfo, object> _implementation;

        public MockInterceptor(Func<IInvocationInfo, object> implementation)
        {
            _implementation = implementation;
        }

        public bool Called { get; set; }

        #region IInterceptor Members

        public object Intercept(IInvocationInfo info)
        {
            Called = true;
            return _implementation(info);
        }

        #endregion
    }
}