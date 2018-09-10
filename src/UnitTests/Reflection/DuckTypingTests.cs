using LinFu.IoC;
using LinFu.Proxy;
using Moq;
using Xunit;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.Reflection
{
    public class DuckTypingTests
    {
        [Fact]
        public void ShouldBeAbleToRedirectInterfaceCallToTarget()
        {
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");

            // The duck should call the implementation instance
            var mock = new Mock<SampleDuckTypeImplementation>();
            mock.Expect(i => i.DoSomething());

            object target = mock.Object;

            var sampleService = target.CreateDuck<ISampleService>();
            sampleService.DoSomething();

            mock.VerifyAll();
        }
    }
}