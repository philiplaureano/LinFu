using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration.Interfaces;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class PropertyInjectionTests : BaseTestFixture
    {
        [Test]
        public void ShouldDetermineWhichPropertiesShouldBeInjected()
        {
            var targetType = typeof(SampleClassWithInjectionProperties);
            var targetProperty = targetType.GetProperty("SomeProperty");
            Assert.IsNotNull(targetProperty);

            // Load the property injection filter by default
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var filter = container.GetService<IMemberInjectionFilter<PropertyInfo>>();

            Assert.IsNotNull(filter);

            // The filter should return the targetProperty
            var properties = filter.GetInjectableMembers(targetType);
            Assert.IsTrue(properties.Count() > 0);

            var result = properties.First();
            Assert.AreEqual(targetProperty, result);
        }

        [Test]
        public void ShouldSetPropertyValue()
        {
            var targetType = typeof(SampleClassWithInjectionProperties);
            var targetProperty = targetType.GetProperty("SomeProperty");
            Assert.IsNotNull(targetProperty);

            // Configure the target
            var instance = new SampleClassWithInjectionProperties();

            // This is the service that should be assigned
            // to the SomeProperty property
            object service = new SampleClass();

            // Initialize the container
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            IPropertySetter setter = container.GetService<IPropertySetter>();
            Assert.IsNotNull(setter);

            setter.Set(instance, targetProperty, service);

            Assert.IsNotNull(instance.SomeProperty);
            Assert.AreSame(service, instance.SomeProperty);
        }

        [Test]
        public void ShouldAutoInjectPropertyWithoutCustomAttribute()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithUnmarkedInjectionProperties();

            // Initialize the container with the dummy service
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<ISampleService>("MyService").Using(c => instance).OncePerRequest();

            // Enable automatic property injection for every property
            container.SetCustomPropertyInjectionAttribute(null);

            // Get the service instance
            var result = container.GetService<ISampleService>("MyService");
            Assert.AreSame(result, instance);

            // Ensure that the injection occurred
            Assert.IsNotNull(instance.SomeProperty);
            Assert.IsInstanceOfType(typeof (SampleClass), instance.SomeProperty);
        }
        [Test]
        public void ShouldAutoInjectProperty()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithInjectionProperties();

            // Initialize the container
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<ISampleService>("MyService").Using(c => instance).OncePerRequest();
            
            var result = container.GetService<ISampleService>("MyService");
            Assert.AreSame(result, instance);

            // On initialization, the instance.SomeProperty value
            // should be a SampleClass type
            Assert.IsNotNull(instance.SomeProperty);
            Assert.IsInstanceOfType(typeof(SampleClass), instance.SomeProperty);
        }

        [Test]
        public void ShouldAutoInjectServiceListIntoArrayDependency()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithArrayPropertyDependency();

            // Initialize the container
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<SampleClassWithArrayPropertyDependency>().Using(c => instance).OncePerRequest();

            var result = container.GetService<SampleClassWithArrayPropertyDependency>();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Property);

            var serviceCount = result.Property.Count();
            Assert.IsTrue(serviceCount > 0);
        }
    }
}
