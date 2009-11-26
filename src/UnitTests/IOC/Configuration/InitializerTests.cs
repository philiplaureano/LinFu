using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using NUnit.Framework;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class InitializerTests
    {
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
    }
}
