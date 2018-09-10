using System;
using System.Linq;
using System.Reflection;
using LinFu.IoC;
using LinFu.IoC.Configuration.Interfaces;
using Moq;
using Xunit;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    public class PropertyInjectionTests : BaseTestFixture
    {
        [Fact]
        public void ShouldAutoInjectClassCreatedWithAutoCreate()
        {
            // Configure the container
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");

            var sampleService = new Mock<ISampleService>();
            container.AddService(sampleService.Object);

            var instance =
                (SampleClassWithInjectionProperties) container.AutoCreate(typeof(SampleClassWithInjectionProperties));

            // The container should initialize the SomeProperty method to match the mock ISampleService instance
            Assert.NotNull(instance.SomeProperty);
            Assert.Same(instance.SomeProperty, sampleService.Object);
        }

        [Fact]
        public void ShouldAutoInjectProperty()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithInjectionProperties();

            // Initialize the container
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<ISampleService>("MyService").Using(c => instance).OncePerRequest();

            var result = container.GetService<ISampleService>("MyService");
            Assert.Same(result, instance);

            // On initialization, the instance.SomeProperty value
            // should be a SampleClass type
            Assert.NotNull(instance.SomeProperty);
            Assert.Equal(typeof(SampleClass), instance.SomeProperty?.GetType());
        }

        [Fact]
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
            Assert.Same(result, instance);

            // Ensure that the injection occurred
            Assert.NotNull(instance.SomeProperty);
            Assert.Equal(typeof(SampleClass), instance.SomeProperty?.GetType());
        }

        [Fact]
        public void ShouldAutoInjectServiceListIntoArrayDependency()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var instance = new SampleClassWithArrayPropertyDependency();

            // Initialize the container
            container.Inject<ISampleService>().Using<SampleClass>().OncePerRequest();
            container.Inject<SampleClassWithArrayPropertyDependency>().Using(c => instance).OncePerRequest();

            var result = container.GetService<SampleClassWithArrayPropertyDependency>();

            Assert.NotNull(result);
            Assert.NotNull(result.Property);

            var serviceCount = result.Property.Count();
            Assert.True(serviceCount > 0);
        }

        [Fact]
        public void ShouldDetermineWhichPropertiesShouldBeInjected()
        {
            var targetType = typeof(SampleClassWithInjectionProperties);
            var targetProperty = targetType.GetProperty("SomeProperty");
            Assert.NotNull(targetProperty);

            // Load the property injection filter by default
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var filter = container.GetService<IMemberInjectionFilter<PropertyInfo>>();

            Assert.NotNull(filter);

            // The filter should return the targetProperty
            var properties = filter.GetInjectableMembers(targetType).ToArray();
            Assert.True(properties.Any());

            var result = properties.First();
            Assert.Equal(targetProperty, result);
        }

        [Fact]
        public void ShouldSetPropertyValue()
        {
            var targetType = typeof(SampleClassWithInjectionProperties);
            var targetProperty = targetType.GetProperty("SomeProperty");
            Assert.NotNull(targetProperty);

            // Configure the target
            var instance = new SampleClassWithInjectionProperties();

            // This is the service that should be assigned
            // to the SomeProperty property
            object service = new SampleClass();

            // Initialize the container
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var setter = container.GetService<IPropertySetter>();
            Assert.NotNull(setter);

            setter.Set(instance, targetProperty, service);

            Assert.NotNull(instance.SomeProperty);
            Assert.Same(service, instance.SomeProperty);
        }
    }
}