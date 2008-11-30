using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LinFu.IoC;
using LinFu.IoC.Configuration;
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
            Assert.IsInstanceOfType(typeof(SampleClass), instance.Property);
        }
    }
}
