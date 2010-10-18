using LinFu.IoC;
using LinFu.Proxy;
using Moq;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.Reflection
{
    [TestFixture]
    public class DuckTypingTests
    {
        [Test]
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