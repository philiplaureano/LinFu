using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
using Moq;
using Xunit;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    public class ResolutionTests
    {
        private static ServiceContainer GetContainerWithMockSampleServices()
        {
            var mockSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            // Add a bunch of dummy services
            for (var i = 0; i < 10; i++)
            {
                var serviceName = string.Format("Service{0}", i + 1);
                container.AddService(serviceName, mockSampleService.Object);
            }

            var services = container.GetServices<ISampleService>();
            Assert.True(services.Count() == 10);

            foreach (var service in services) Assert.Same(mockSampleService.Object, service);

            return container;
        }

        [Fact]
        public void ShouldAutoCreateClassWithServiceArrayAsConstructorArgument()
        {
            var container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate(typeof(SampleClassWithServiceArrayAsConstructorArgument))
                as SampleClassWithServiceArrayAsConstructorArgument;

            Assert.NotNull(result);
            Assert.True(result.Services.Length > 0);
        }

        [Fact]
        public void ShouldAutoCreateClassWithServiceEnumerableAsConstructorArgument()
        {
            var container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate(typeof(SampleClassWithServiceEnumerableAsConstructorArgument))
                as SampleClassWithServiceEnumerableAsConstructorArgument;

            Assert.NotNull(result);
            Assert.NotNull(result.Services);
            Assert.True(result.Services.Count() > 0);
        }

        [Fact]
        public void ShouldBeAbleToAutoCreateClassUsingGenericAutoCreateCall()
        {
            var container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate<SampleClassWithServiceArrayAsConstructorArgument>();

            Assert.NotNull(result);
            Assert.True(result.Services.Length > 0);
        }

        [Fact]
        public void ShouldCallStronglyTypedFunctorInsteadOfActualFactory()
        {
            var container = new ServiceContainer();

            Func<int, int, int> addOperation1 = (a, b) => a + b;
            container.AddService("Add", addOperation1);

            Func<int, int, int, int> addOperation2 = (a, b, c) => a + b + c;
            container.AddService("Add", addOperation2);

            Func<int, int, int, int, int> addOperation3 = (a, b, c, d) => a + b + c + d;
            container.AddService("Add", addOperation3);

            Assert.Equal(2, container.GetService<int>("Add", 1, 1));
            Assert.Equal(3, container.GetService<int>("Add", 1, 1, 1));
            Assert.Equal(4, container.GetService<int>("Add", 1, 1, 1, 1));
        }

        [Fact]
        public void ShouldConstructParametersFromContainer()
        {
            var targetConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new[]
            {
                typeof
                (
                    ISampleService
                ),
                typeof
                (
                    ISampleService
                )
            });

            // Initialize the container
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();
            container.AddService(mockSampleService.Object);
            container.AddService<IArgumentResolver>(new ArgumentResolver());

            // Generate the arguments using the target constructor
            var arguments = targetConstructor.ResolveArgumentsFrom(container);
            Assert.Same(arguments[0], mockSampleService.Object);
            Assert.Same(arguments[1], mockSampleService.Object);
        }

        [Fact]
        public void ShouldConvertParameterInfoIntoPredicateThatChecksIfParameterTypeExistsAsService()
        {
            // Initialize the container so that it contains
            // an instance of ISampleService
            var mockSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            container.AddService(mockSampleService.Object);

            var constructor = typeof(SampleClassWithSingleArgumentConstructor)
                .GetConstructor(new[] {typeof(ISampleService)});

            var parameters = constructor.GetParameters();
            var firstParameter = parameters.First();

            Assert.NotNull(firstParameter);

            var mustExist = firstParameter.ParameterType.MustExistInContainer();
            Assert.True(mustExist(container));
        }

        [Fact]
        public void ShouldConvertTypeIntoPredicateThatChecksIfTypeExistsInContainerAsEnumerableSetOfServices()
        {
            var container = GetContainerWithMockSampleServices();

            var predicate = typeof(IEnumerable<ISampleService>)
                .ExistsAsEnumerableSetOfServices();

            Assert.True(predicate(container));
        }

        [Fact]
        public void ShouldConvertTypeIntoPredicateThatChecksIfTypeExistsInContainerAsServiceArray()
        {
            var container = GetContainerWithMockSampleServices();

            var predicate = typeof(ISampleService[])
                .ExistsAsServiceArray();

            Assert.True(predicate(container));
        }

        [Fact]
        public void ShouldCreateTypeWithAdditionalParameters()
        {
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();

            // Add an ISampleService instance
            container.AddService(mockSampleService.Object);

            var resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            Assert.NotNull(resolver);

            var instance =
                container.AutoCreate(typeof(SampleClassWithAdditionalArgument), 42) as
                    SampleClassWithAdditionalArgument;
            Assert.NotNull(instance);
            Assert.True(instance.Argument == 42);
        }

        [Fact]
        public void ShouldInjectConstructorWithNamedParameterTypes()
        {
            var mockDefaultSampleService = new Mock<ISampleService>();
            var mockOtherSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            // Add the default service
            container.AddService(mockDefaultSampleService.Object);

            // Add the expected service instance
            container.AddService("OtherService", mockOtherSampleService.Object);

            var serviceInstance =
                (SampleClassWithNamedParameters) container.AutoCreate(typeof(SampleClassWithNamedParameters));

            Assert.Equal(mockOtherSampleService.Object, serviceInstance.ServiceInstance);
        }

        [Fact]
        public void ShouldInstantiateClassWithNonServiceArguments()
        {
            var container = new ServiceContainer();
            container.AddDefaultServices();

            container.AddService(typeof(SampleClassWithNonServiceArgument), typeof(SampleClassWithNonServiceArgument));

            var text = "Hello, World!";
            string serviceName = null;
            var result = container.GetService<SampleClassWithNonServiceArgument>(serviceName, text);

            Assert.NotNull(result);
            Assert.True(result.Value == text);
        }

        [Fact]
        public void ShouldInstantiateClassWithServiceArrayAsConstructorArgument()
        {
            var container = GetContainerWithMockSampleServices();
            container.AddService(typeof(SampleClassWithServiceArrayAsConstructorArgument),
                typeof(SampleClassWithServiceArrayAsConstructorArgument));

            var result = container.GetService(typeof(SampleClassWithServiceArrayAsConstructorArgument)) as
                SampleClassWithServiceArrayAsConstructorArgument;

            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.NotNull(result.Services);
            Assert.True(result.Services.Count() > 0);
        }

        [Fact]
        public void ShouldInstantiateClassWithServiceEnumerableAsConstructorArgument()
        {
            var container = GetContainerWithMockSampleServices();
            container.AddService(typeof(SampleClassWithServiceEnumerableAsConstructorArgument),
                typeof(SampleClassWithServiceEnumerableAsConstructorArgument));

            var result =
                container.GetService(typeof(SampleClassWithServiceEnumerableAsConstructorArgument)) as
                    SampleClassWithServiceEnumerableAsConstructorArgument;

            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.NotNull(result.Services);
            Assert.True(result.Services.Count() > 0);
        }

        [Fact]
        public void ShouldInstantiateObjectWithConstructorAndArguments()
        {
            var targetConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new[]
            {
                typeof
                (
                    ISampleService
                ),
                typeof
                (
                    ISampleService
                )
            });

            // Create the method arguments
            var mockSampleService = new Mock<ISampleService>();
            var arguments = new object[] {mockSampleService.Object, mockSampleService.Object};

            // Initialize the container
            IServiceContainer container = new ServiceContainer();
            container.AddDefaultServices();

            var constructorInvoke = container.GetService<IMethodInvoke<ConstructorInfo>>();
            var result = constructorInvoke.Invoke(null, targetConstructor, arguments);

            Assert.NotNull(result);
            Assert.Equal(typeof(SampleClassWithMultipleConstructors), result.GetType());
        }

        [Fact]
        public void ShouldReportThatServiceExistsForStronglyTypedFunctor()
        {
            var container = new ServiceContainer();

            Func<int, int, int> addOperation1 = (a, b) => a + b;
            container.AddService("Add", addOperation1);

            Assert.True(container.Contains("Add", typeof(int), 1, 1));
        }

        [Fact]
        public void ShouldResolveClassWithMultipleNonServiceArgumentConstructors()
        {
            IServiceContainer container = new ServiceContainer();
            container.AddDefaultServices();
            container.AddService("ClassWithMultipleNonServiceArgumentConstructors",
                typeof(ISampleService), typeof(SampleClassWithMultipleNonServiceArgumentConstructors),
                LifecycleType.OncePerRequest);

            // Match the correct constructor
            var sampleService = container.GetService<ISampleService>("ClassWithMultipleNonServiceArgumentConstructors",
                "test", 42, SampleEnum.One, (decimal) 3.14, 42);
            Assert.NotNull(sampleService);
        }

        [Fact]
        public void ShouldResolveConstructorWithAdditionalArgument()
        {
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();

            // Add an ISampleService instance
            container.AddService(mockSampleService.Object);
            container.AddDefaultServices();

            var resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            Assert.NotNull(resolver);

            // The resolver should return the constructor
            // with the following signature: Constructor(ISampleService, int)
            var expectedConstructor =
                typeof(SampleClassWithAdditionalArgument).GetConstructor(new[] {typeof(ISampleService), typeof(int)});
            Assert.NotNull(expectedConstructor);


            var context = new MethodFinderContext(42);
            var result = resolver.ResolveFrom(typeof(SampleClassWithAdditionalArgument), container, context);
            Assert.Same(expectedConstructor, result);
        }

        [Fact]
        public void ShouldResolveConstructorWithMostResolvableParametersFromContainer()
        {
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();

            // Add an ISampleService instance
            container.AddService(mockSampleService.Object);
            container.AddDefaultServices();
            var resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            Assert.NotNull(resolver);

            // The resolver should return the constructor with two ISampleService parameters
            var expectedConstructor =
                typeof(SampleClassWithMultipleConstructors).GetConstructor(new[]
                {
                    typeof(ISampleService),
                    typeof(ISampleService)
                });
            Assert.NotNull(expectedConstructor);

            var finderContext = new MethodFinderContext(new Type[0], new object[0], null);
            var result = resolver.ResolveFrom(typeof(SampleClassWithMultipleConstructors), container,
                finderContext);
            Assert.Same(expectedConstructor, result);
        }
    }
}