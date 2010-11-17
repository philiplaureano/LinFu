using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
using Moq;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class ResolutionTests
    {
        private static ServiceContainer GetContainerWithMockSampleServices()
        {
            var mockSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            // Add a bunch of dummy services
            for (int i = 0; i < 10; i++)
            {
                string serviceName = string.Format("Service{0}", i + 1);
                container.AddService(serviceName, mockSampleService.Object);
            }

            IEnumerable<ISampleService> services = container.GetServices<ISampleService>();
            Assert.IsTrue(services.Count() == 10);

            foreach (ISampleService service in services)
            {
                Assert.AreSame(mockSampleService.Object, service);
            }

            return container;
        }

        [Test]
        public void ShouldAutoCreateClassWithServiceArrayAsConstructorArgument()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate(typeof(SampleClassWithServiceArrayAsConstructorArgument))
                         as SampleClassWithServiceArrayAsConstructorArgument;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Services.Length > 0);
        }

        [Test]
        public void ShouldAutoCreateClassWithServiceEnumerableAsConstructorArgument()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate(typeof(SampleClassWithServiceEnumerableAsConstructorArgument))
                         as SampleClassWithServiceEnumerableAsConstructorArgument;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Services);
            Assert.IsTrue(result.Services.Count() > 0);
        }

        [Test]
        public void ShouldBeAbleToAutoCreateClassUsingGenericAutoCreateCall()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate<SampleClassWithServiceArrayAsConstructorArgument>();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Services.Length > 0);
        }

        [Test]
        public void ShouldCallStronglyTypedFunctorInsteadOfActualFactory()
        {
            var container = new ServiceContainer();

            Func<int, int, int> addOperation1 = (a, b) => a + b;
            container.AddService("Add", addOperation1);

            Func<int, int, int, int> addOperation2 = (a, b, c) => a + b + c;
            container.AddService("Add", addOperation2);

            Func<int, int, int, int, int> addOperation3 = (a, b, c, d) => a + b + c + d;
            container.AddService("Add", addOperation3);

            Assert.AreEqual(2, container.GetService<int>("Add", 1, 1));
            Assert.AreEqual(3, container.GetService<int>("Add", 1, 1, 1));
            Assert.AreEqual(4, container.GetService<int>("Add", 1, 1, 1, 1));
        }

        [Test]
        public void ShouldConstructParametersFromContainer()
        {
            ConstructorInfo targetConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new[]
                                                                                                                {
                                                                                                                    typeof
                                                                                                                        (
                                                                                                                        ISampleService
                                                                                                                        )
                                                                                                                    ,
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
            object[] arguments = targetConstructor.ResolveArgumentsFrom(container);
            Assert.AreSame(arguments[0], mockSampleService.Object);
            Assert.AreSame(arguments[1], mockSampleService.Object);
        }

        [Test]
        public void ShouldConvertParameterInfoIntoPredicateThatChecksIfParameterTypeExistsAsService()
        {
            // Initialize the container so that it contains
            // an instance of ISampleService
            var mockSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            container.AddService(mockSampleService.Object);

            ConstructorInfo constructor = typeof(SampleClassWithSingleArgumentConstructor)
                .GetConstructor(new[] { typeof(ISampleService) });

            ParameterInfo[] parameters = constructor.GetParameters();
            ParameterInfo firstParameter = parameters.First();

            Assert.IsNotNull(firstParameter);

            Func<IServiceContainer, bool> mustExist = firstParameter.ParameterType.MustExistInContainer();
            Assert.IsTrue(mustExist(container));
        }

        [Test]
        public void ShouldConvertTypeIntoPredicateThatChecksIfTypeExistsInContainerAsEnumerableSetOfServices()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();

            Func<IServiceContainer, bool> predicate = typeof(IEnumerable<ISampleService>)
                .ExistsAsEnumerableSetOfServices();

            Assert.IsTrue(predicate(container));
        }

        [Test]
        public void ShouldConvertTypeIntoPredicateThatChecksIfTypeExistsInContainerAsServiceArray()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();

            Func<IServiceContainer, bool> predicate = typeof(ISampleService[])
                .ExistsAsServiceArray();

            Assert.IsTrue(predicate(container));
        }

        [Test]
        public void ShouldCreateTypeWithAdditionalParameters()
        {
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();

            // Add an ISampleService instance
            container.AddService(mockSampleService.Object);

            var resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            Assert.IsNotNull(resolver);

            var instance =
                container.AutoCreate(typeof(SampleClassWithAdditionalArgument), 42) as
                SampleClassWithAdditionalArgument;
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.Argument == 42);
        }

        [Test]
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
                (SampleClassWithNamedParameters)container.AutoCreate(typeof(SampleClassWithNamedParameters));

            Assert.AreEqual(mockOtherSampleService.Object, serviceInstance.ServiceInstance);
        }

        [Test]
        public void ShouldInstantiateClassWithNonServiceArguments()
        {
            var container = new ServiceContainer();
            container.AddDefaultServices();

            container.AddService(typeof(SampleClassWithNonServiceArgument), typeof(SampleClassWithNonServiceArgument));

            string text = "Hello, World!";
            string serviceName = null;
            var result = container.GetService<SampleClassWithNonServiceArgument>(serviceName, text);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value == text);
        }

        [Test]
        public void ShouldInstantiateClassWithServiceArrayAsConstructorArgument()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();
            container.AddService(typeof(SampleClassWithServiceArrayAsConstructorArgument),
                                 typeof(SampleClassWithServiceArrayAsConstructorArgument));

            var result = container.GetService(typeof(SampleClassWithServiceArrayAsConstructorArgument)) as
                         SampleClassWithServiceArrayAsConstructorArgument;

            Assert.IsNotNull(result);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Services);
            Assert.IsTrue(result.Services.Count() > 0);
        }

        [Test]
        public void ShouldInstantiateClassWithServiceEnumerableAsConstructorArgument()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();
            container.AddService(typeof(SampleClassWithServiceEnumerableAsConstructorArgument),
                                 typeof(SampleClassWithServiceEnumerableAsConstructorArgument));

            var result =
                container.GetService(typeof(SampleClassWithServiceEnumerableAsConstructorArgument)) as
                SampleClassWithServiceEnumerableAsConstructorArgument;

            Assert.IsNotNull(result);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Services);
            Assert.IsTrue(result.Services.Count() > 0);
        }

        [Test]
        public void ShouldInstantiateObjectWithConstructorAndArguments()
        {
            ConstructorInfo targetConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new[]
                                                                                                                {
                                                                                                                    typeof
                                                                                                                        (
                                                                                                                        ISampleService
                                                                                                                        )
                                                                                                                    ,
                                                                                                                    typeof
                                                                                                                        (
                                                                                                                        ISampleService
                                                                                                                        )
                                                                                                                });

            // Create the method arguments
            var mockSampleService = new Mock<ISampleService>();
            var arguments = new object[] { mockSampleService.Object, mockSampleService.Object };

            // Initialize the container
            IServiceContainer container = new ServiceContainer();
            container.AddDefaultServices();

            var constructorInvoke = container.GetService<IMethodInvoke<ConstructorInfo>>();
            object result = constructorInvoke.Invoke(null, targetConstructor, arguments);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(SampleClassWithMultipleConstructors), result);
        }

        [Test]
        public void ShouldReportThatServiceExistsForStronglyTypedFunctor()
        {
            var container = new ServiceContainer();

            Func<int, int, int> addOperation1 = (a, b) => a + b;
            container.AddService("Add", addOperation1);

            Assert.IsTrue(container.Contains("Add", typeof(int), 1, 1));
        }

        [Test]
        public void ShouldResolveClassWithMultipleNonServiceArgumentConstructors()
        {
            IServiceContainer container = new ServiceContainer();
            container.AddDefaultServices();
            container.AddService("ClassWithMultipleNonServiceArgumentConstructors",
                                 typeof(ISampleService), typeof(SampleClassWithMultipleNonServiceArgumentConstructors),
                                 LifecycleType.OncePerRequest);

            // Match the correct constructor
            var sampleService = container.GetService<ISampleService>("ClassWithMultipleNonServiceArgumentConstructors",
                                                                     "test", 42, SampleEnum.One, (decimal)3.14, 42);
            Assert.IsNotNull(sampleService);
        }

        [Test]
        public void ShouldResolveConstructorWithAdditionalArgument()
        {
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();

            // Add an ISampleService instance
            container.AddService(mockSampleService.Object);
            container.AddDefaultServices();

            var resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            Assert.IsNotNull(resolver);

            // The resolver should return the constructor
            // with the following signature: Constructor(ISampleService, int)
            ConstructorInfo expectedConstructor =
                typeof(SampleClassWithAdditionalArgument).GetConstructor(new[] { typeof(ISampleService), typeof(int) });
            Assert.IsNotNull(expectedConstructor);


            var context = new MethodFinderContext(42);
            ConstructorInfo result = resolver.ResolveFrom(typeof(SampleClassWithAdditionalArgument), container, context);
            Assert.AreSame(expectedConstructor, result);
        }
        
        [Test]
        public void ShouldResolveConstructorWithMostResolvableParametersFromContainer()
        {
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();

            // Add an ISampleService instance
            container.AddService(mockSampleService.Object);
            container.AddDefaultServices();
            var resolver = container.GetService<IMemberResolver<ConstructorInfo>>();
            Assert.IsNotNull(resolver);

            // The resolver should return the constructor with two ISampleService parameters
            ConstructorInfo expectedConstructor =
                typeof(SampleClassWithMultipleConstructors).GetConstructor(new[]
                                                                                {
                                                                                    typeof (ISampleService),
                                                                                    typeof (ISampleService)
                                                                                });
            Assert.IsNotNull(expectedConstructor);

            var finderContext = new MethodFinderContext(new Type[0], new object[0], null);
            ConstructorInfo result = resolver.ResolveFrom(typeof(SampleClassWithMultipleConstructors), container,
                                                          finderContext);
            Assert.AreSame(expectedConstructor, result);
        }
    }
}