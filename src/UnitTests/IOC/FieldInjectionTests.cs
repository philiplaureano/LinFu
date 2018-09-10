using System;
using LinFu.IoC;
using Xunit;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    public class FieldInjectionTests : BaseTestFixture
    {
        [Fact]
        public void ShouldAutoInjectField()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithInjectionField();

            // Initialize the container
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<ISampleService>("MyService").Using(c => instance).OncePerRequest();

            var result = container.GetService<ISampleService>("MyService");
            Assert.Same(result, instance);

            // On initialization, the instance.SomeField value
            // should be a SampleClass type
            Assert.NotNull(instance.SomeField);
            Assert.Equal(typeof(SampleClass), instance?.SomeField?.GetType());
        }
    }
}