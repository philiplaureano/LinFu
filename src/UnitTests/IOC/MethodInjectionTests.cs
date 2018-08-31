using System;
using LinFu.IoC;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class MethodInjectionTests : BaseTestFixture
    {
        [Test]
        public void ShouldAutoInjectMethod()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithInjectionMethod();

            // Initialize the container
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<ISampleService>("MyService").Using(c => instance).OncePerRequest();

            var result = container.GetService<ISampleService>("MyService");
            Assert.AreSame(result, instance);

            // On initialization, the instance.Property value
            // should be a SampleClass type
            Assert.IsNotNull(instance.Property);
            Assert.AreEqual(typeof(SampleClass), instance.Property?.GetType());
        }
    }
}