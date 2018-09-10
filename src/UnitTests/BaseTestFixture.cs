

using System;

namespace LinFu.UnitTests
{
    public abstract class BaseTestFixture : IDisposable
    {
        private bool _disposed = false;
        protected BaseTestFixture()
        {
            var self = this;
            self.Init();
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                Term();
                _disposed = true;
            }
        }

        protected virtual void Init()
        {
        }

        protected virtual void Term()
        {
        }
    }
}