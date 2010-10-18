using System;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using NUnit.Framework;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class InitializerTests
    {
        public class InitializableObject : IInitialize
        {
            public bool InitializeCalled { get; set; }

            #region IInitialize Members

            public void Initialize(IServiceContainer source)
            {
                InitializeCalled = true;
            }

            #endregion
        }

        public class TestServiceContainer : ServiceContainer
        {
            public TestServiceContainer()
            {
                this.AddService(typeof (InitializableObject), typeof (InitializableObject));
            }
        }

        [Test]
        public void InitializerDoesNotHoldRerenceToInitializedObjects()
        {
            var container = new TestServiceContainer();

            var initializable = container.GetService<InitializableObject>();
            Assert.IsTrue(initializable.InitializeCalled);
            var weakRef = new WeakReference(initializable);
            Assert.IsTrue(weakRef.IsAlive);

            initializable = null;
            GC.Collect(0, GCCollectionMode.Forced);
            Assert.IsFalse(weakRef.IsAlive);
        }
    }
}