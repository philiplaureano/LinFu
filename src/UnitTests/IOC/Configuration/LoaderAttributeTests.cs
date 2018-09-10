using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Loaders;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;
using Moq;
using Xunit;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    public class LoaderAttributeTests
    {
        private static void TestFactoryConverterWith<TFactory>(string serviceName, Type serviceType,
            Type implementingType, ITypeLoader loader)
            where TFactory : IFactory
        {
            // The loader should initialize the container with
            // the particular factory type
            var mockContainer = new Mock<IServiceContainer>();
            mockContainer.Expect(container =>
                container.AddFactory(serviceName, serviceType, It.IsAny<IEnumerable<Type>>(),
                    It.Is<IFactory>(
                        f => f != null && f is TFactory || f is FunctorFactory)));

            var factoryActions = loader.Load(implementingType).ToArray();
            Assert.NotNull(factoryActions);
            Assert.True(factoryActions.Count() == 1);

            // There must be at least one factory from
            // the result list
            var firstResult = factoryActions.FirstOrDefault();
            Assert.NotNull(firstResult);


            firstResult(mockContainer.Object);

            mockContainer.VerifyAll();
        }

        [Fact]
        public void FactoryAttributeLoaderMustInjectOpenGenericServiceTypeIntoContainer()
        {
            var mockContainer = new Mock<IServiceContainer>();
            var serviceType = typeof(ISampleGenericService<>);
            var mockPostProcessors = new Mock<IList<IPostProcessor>>();
            var mockPreProcessors = new Mock<IList<IPreProcessor>>();

            ITypeLoader loader = new FactoryAttributeLoader();
            var actions = loader.Load(typeof(SampleOpenGenericFactory));


            // The loader should load the mock container
            // using the generic open type as the service type
            mockContainer.Expect(container => container.PostProcessors)
                .Returns(mockPostProcessors.Object);

            mockContainer.Expect(container => container.PreProcessors)
                .Returns(mockPreProcessors.Object);

            mockContainer.Expect(container => container.AddFactory(null,
                serviceType, It.IsAny<IEnumerable<Type>>(),
                It.IsAny<SampleOpenGenericFactory>()));

            // The postprocessor list should have an additional element added
            mockPostProcessors.Expect(p => p.Add(It.IsAny<IPostProcessor>()));

            // The preprocessor list should have an additional element added as well
            mockPreProcessors.Expect(p => p.Add(It.IsAny<IPreProcessor>()));

            Action<IServiceContainer> applyActions =
                container =>
                {
                    foreach (var action in actions) action(container);
                };

            applyActions(mockContainer.Object);

            // Apply the actions to a real container and verify
            // it with the expected service instance
            var realContainer = new ServiceContainer();
            applyActions(realContainer);
        }

        [Fact]
        public void FactoryAttributeLoaderMustInjectStronglyTypedFactoryIntoContainer()
        {
            var container = new ServiceContainer();
            var serviceType = typeof(ISampleService);

            ITypeLoader loader = new FactoryAttributeLoader();
            var actions = loader.Load(typeof(SampleStronglyTypedFactory));

            // The factory loader should return a set of actions
            // that will inject that custom factory into the container
            // itself
            foreach (var action in actions) action(container);

            Assert.True(container.Contains(serviceType));
        }

        [Fact]
        public void FactoryAttributeLoaderMustInjectUnnamedCustomFactoryIntoContainer()
        {
            var mockContainer = new Mock<IServiceContainer>();
            var mockPreProcessors = new Mock<IList<IPreProcessor>>();
            var serviceType = typeof(ISampleService);
            string serviceName = null;

            // The container should add the expected
            // factory type
            mockContainer.Expect(
                container => container.AddFactory(serviceName, serviceType, It.IsAny<IEnumerable<Type>>(),
                    It.IsAny<SampleFactory>()));

            // The factory attribute loader will add the custom
            // factory to the preprocessors collection
            mockContainer.Expect(container => container.PreProcessors)
                .Returns(mockPreProcessors.Object);

            mockPreProcessors.Expect(p => p.Add(It.IsAny<IPreProcessor>()));

            ITypeLoader loader = new FactoryAttributeLoader();
            var actions = loader.Load(typeof(SampleFactory));

            // The factory loader should return a set of actions
            // that will inject that custom factory into the container
            // itself
            foreach (var action in actions) action(mockContainer.Object);

            mockContainer.VerifyAll();
        }

        [Fact]
        public void LoaderMustLoadSingletonTypesAndThoseTypesMustBeTheSameInstance()
        {
            var location = typeof(SamplePostProcessor).Assembly.Location ?? string.Empty;
            var loader = new Loader();
            var directory = Path.GetDirectoryName(location);

            // Load the default plugins first
            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            // Load the sample library
            loader.LoadDirectory(directory, Path.GetFileName(location));

            var filename = Path.Combine(directory, location);
            Assert.True(File.Exists(filename));

            var container = new ServiceContainer();

            loader.LoadInto(container);

            var first = container.GetService<ISampleService>("First");
            var second = container.GetService<ISampleService>("First");

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.Same(first, second);
        }

        [Fact]
        public void LoaderMustLoadTheCorrectOncePerRequestTypes()
        {
            var location = typeof(SamplePostProcessor).Assembly.Location ?? string.Empty;
            var loader = new Loader();
            var directory = Path.GetDirectoryName(location);

            // Load the default plugins first
            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            // Load the sample library
            loader.LoadDirectory(directory, Path.GetFileName(location));

            var filename = Path.Combine(directory, location);
            Assert.True(File.Exists(filename));

            var container = new ServiceContainer();

            loader.LoadInto(container);

            var first = container.GetService<ISampleService>("FirstOncePerRequestService");
            var second = container.GetService<ISampleService>("SecondOncePerRequestService");

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.True(first.GetType() == typeof(FirstOncePerRequestService));
            Assert.True(second.GetType() == typeof(SecondOncePerRequestService));
        }

        [Fact]
        public void LoaderMustLoadTheCorrectSingletonTypes()
        {
            var location = typeof(SamplePostProcessor).Assembly.Location ?? string.Empty;
            var loader = new Loader();
            var directory = Path.GetDirectoryName(location);

            // Load the default plugins first
            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            // Load the sample library
            loader.LoadDirectory(directory, Path.GetFileName(location));

            var filename = Path.Combine(directory, location);
            Assert.True(File.Exists(filename));

            var container = new ServiceContainer();

            loader.LoadInto(container);

            var first = container.GetService<ISampleService>("First");
            var second = container.GetService<ISampleService>("Second");

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.True(first.GetType() == typeof(FirstSingletonService));
            Assert.True(second.GetType() == typeof(SecondSingletonService));
        }

        [Fact]
        public void NamedOncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(NamedOncePerRequestSampleService);
            var serviceType = typeof(ISampleService);
            var converter = new ImplementsAttributeLoader();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>("MyService",
                serviceType, implementingType, converter);
        }

        [Fact]
        public void NamedOncePerThreadFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<OncePerThreadFactory<ISampleService>>("MyService",
                typeof(ISampleService),
                typeof(NamedOncePerThreadSampleService),
                new ImplementsAttributeLoader());
        }

        [Fact]
        public void NamedSingletonFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<SingletonFactory<ISampleService>>("MyService",
                typeof(ISampleService),
                typeof(NamedSingletonSampleService),
                new ImplementsAttributeLoader());
        }

        [Fact]
        public void OncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(OncePerRequestSampleService);
            var serviceType = typeof(ISampleService);
            var converter = new ImplementsAttributeLoader();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>(null,
                serviceType, implementingType, converter);
        }

        [Fact]
        public void OncePerThreadFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<OncePerThreadFactory<ISampleService>>(null,
                typeof(ISampleService),
                typeof(OncePerThreadSampleService),
                new ImplementsAttributeLoader());
        }

        [Fact]
        public void ServicesCreatedFromCustomOpenGenericFactoryMustInvokeIInitialize()
        {
            var mockFactory = new Mock<IFactory>();
            var serviceInstance = new SampleGenericImplementation<int>();

            mockFactory.Expect(f => f.CreateInstance(It.IsAny<IFactoryRequest>()))
                .Returns(serviceInstance);

            var container = new ServiceContainer();


            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
            container.AddFactory("MyService", typeof(ISampleGenericService<>), mockFactory.Object);

            var result = container.GetService<ISampleGenericService<int>>("MyService");
            var areSame = ReferenceEquals(serviceInstance, result);

            Assert.Same(serviceInstance, result);

            // Make sure that the IInitialize instance is called
            // on the sample class
            Assert.True(serviceInstance.Called);

            mockFactory.VerifyAll();
        }

        [Fact]
        public void ShouldInjectInternallyVisibleServiceTypeMarkedWithImplementsAttribute()
        {
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");

            var service = container.GetService<ISampleService>("internal");
            Assert.NotNull(service);
        }

        [Fact]
        public void SingletonFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<SingletonFactory<ISampleService>>(null,
                typeof(ISampleService),
                typeof(SingletonSampleService),
                new ImplementsAttributeLoader());
        }
    }
}