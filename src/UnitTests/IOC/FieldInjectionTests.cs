using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;
using LinFu.IoC;
using LinFu.IoC.Configuration;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class FieldInjectionTests : BaseTestFixture 
    {
        [Test]
        public void ShouldAutoInjectField()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithInjectionField();

            // Initialize the container
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<ISampleService>("MyService").Using(c => instance).OncePerRequest();

            var result = container.GetService<ISampleService>("MyService");
            Assert.AreSame(result, instance);

            // On initialization, the instance.SomeField value
            // should be a SampleClass type
            Assert.IsNotNull(instance.SomeField);
            Assert.IsInstanceOfType(typeof(SampleClass), instance.SomeField);
        }
    }
}
