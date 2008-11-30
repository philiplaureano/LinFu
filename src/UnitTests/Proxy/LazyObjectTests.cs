using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interceptors;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.UnitTests.Proxy
{
    [TestFixture]
    public class LazyObjectTests : BaseTestFixture
    {
        [Test]
        public void LazyObjectShouldNeverBeInitialized()
        {
            var container = new ServiceContainer();
            container.AddService<IProxyFactory>(new ProxyFactory());
            container.AddService<IMethodBuilder<MethodInfo>>(new MethodBuilder());

            Assert.IsTrue(container.Contains(typeof(IProxyFactory)));

            var proxyFactory = container.GetService<IProxyFactory>();
            var interceptor = new LazyInterceptor<ISampleService>(() => new SampleLazyService());

            SampleLazyService.Reset();
            // The instance should be uninitialized at this point
            var proxy = proxyFactory.CreateProxy<ISampleService>(interceptor);
            Assert.IsFalse(SampleLazyService.IsInitialized);

            // The instance should be initialized once the method is called
            proxy.DoSomething();
            Assert.IsTrue(SampleLazyService.IsInitialized);
        }
    }
}
