using System;
using System.Threading;
using LinFu.IoC;
using LinFu.IoC.Interfaces;
using Moq;
using Xunit;

namespace LinFu.UnitTests.IOC
{
    public class ScopeTests : BaseTestFixture
    {
        [Fact]
        public void ScopeShouldCallDisposableOnScopedObject()
        {
            var mock = new Mock<IDisposable>();
            mock.Expect(disposable => disposable.Dispose());

            var container = new ServiceContainer();
            container.AddService(mock.Object);

            using (var scope = container.GetService<IScope>())
            {
                // Create the service instance
                var instance = container.GetService<IDisposable>();
            }
        }

        [Fact]
        public void ScopeShouldNeverCallDisposableOnNonScopedObject()
        {
            var mock = new Mock<IDisposable>();
            var container = new ServiceContainer();
            container.AddService(mock.Object);

            using (var scope = container.GetService<IScope>())
            {
            }

            // Create the service instance OUTSIDE the scope
            // Note: this should never be disposed
            var instance = container.GetService<IDisposable>();
        }

        [Fact]
        public void ScopeShouldNeverCallDisposableOnScopedObjectCreatedInAnotherThread()
        {
            var mock = new Mock<IDisposable>();

            var container = new ServiceContainer();
            container.AddService(mock.Object);

            using (var scope = container.GetService<IScope>())
            {
                var signal = new ManualResetEvent(false);
                WaitCallback callback = state =>
                {
                    // Create the service instance
                    var instance = container.GetService<IDisposable>();
                    signal.Set();
                };

                ThreadPool.QueueUserWorkItem(callback);

                // Wait for the thread to execute
                WaitHandle.WaitAny(new WaitHandle[] {signal});
            }

            // The instance should never be disposed
        }
    }
}