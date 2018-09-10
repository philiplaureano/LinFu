using System.Reflection;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interceptors;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using Xunit;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.Proxy
{
    public class LazyObjectTests : BaseTestFixture
    {
        [Fact]
        public void LazyObjectShouldNeverBeInitialized()
        {
            var container = new ServiceContainer();
            container.AddService<IProxyFactory>(new ProxyFactory());
            container.AddService<IMethodBuilder<MethodInfo>>(new MethodBuilder());

            Assert.True(container.Contains(typeof(IProxyFactory)));

            var proxyFactory = container.GetService<IProxyFactory>();
            var interceptor = new LazyInterceptor<ISampleService>(() => new SampleLazyService());

            SampleLazyService.Reset();
            // The instance should be uninitialized at this point
            var proxy = proxyFactory.CreateProxy<ISampleService>(interceptor);
            Assert.False(SampleLazyService.IsInitialized);

            // The instance should be initialized once the method is called
            proxy.DoSomething();
            Assert.True(SampleLazyService.IsInitialized);
        }
    }
}