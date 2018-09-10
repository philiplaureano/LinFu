using System;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using Xunit;

namespace LinFu.UnitTests.IOC.Configuration
{
    public class InitializerTests
    {
        public class InitializableObject : IInitialize
        {
            public bool InitializeCalled { get; set; }


            public void Initialize(IServiceContainer source)
            {
                InitializeCalled = true;
            }
        }

        public class TestServiceContainer : ServiceContainer
        {
            public TestServiceContainer()
            {
                this.AddService(typeof(InitializableObject), typeof(InitializableObject));
            }
        }

        [Fact]
        public void InitializerDoesNotHoldRerenceToInitializedObjects()
        {
            var container = new TestServiceContainer();

            var initializable = container.GetService<InitializableObject>();
            Assert.True(initializable.InitializeCalled);
            var weakRef = new WeakReference(initializable);
            Assert.True(weakRef.IsAlive);

            initializable = null;
            GC.Collect(0, GCCollectionMode.Forced);
            Assert.False(weakRef.IsAlive);
        }
    }
}