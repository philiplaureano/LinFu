using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        [Test]
        public void ShouldConvertParameterInfoIntoPredicateThatChecksIfParameterTypeExistsAsService()
        {
            // Initialize the container so that it contains
            // an instance of ISampleService
            var mockSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            container.AddService(mockSampleService.Object);

            var constructor = typeof(SampleClassWithSingleArgumentConstructor)
                .GetConstructor(new[] { typeof(ISampleService) });

            var parameters = constructor.GetParameters();
            var firstParameter = parameters.First();

            Assert.IsNotNull(firstParameter);

            Func<IServiceContainer, bool> mustExist = firstParameter.ParameterType.MustExistInContainer();
            Assert.IsTrue(mustExist(container));
        }

        [Test]
        public void ShouldConvertTypeIntoPredicateThatChecksIfTypeExistsInContainerAsServiceArray()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();

            var predicate = typeof(ISampleService[])
                .ExistsAsServiceArray();

            Assert.IsTrue(predicate(container));
        }

        [Test]
        public void ShouldAutoCreateClassWithServiceArrayAsConstructorArgument()
        {
            var container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate(typeof(SampleClassWithServiceArrayAsConstructorArgument))
                         as SampleClassWithServiceArrayAsConstructorArgument;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Services.Length > 0);
        }

        [Test]
        public void ShouldAutoCreateClassWithServiceEnumerableAsConstructorArgument()
        {
            var container = GetContainerWithMockSampleServices();
            var result = container.AutoCreate(typeof(SampleClassWithServiceEnumerableAsConstructorArgument))
                         as SampleClassWithServiceEnumerableAsConstructorArgument;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Services);
            Assert.IsTrue(result.Services.Count() > 0);
        }

        [Test]
        public void ShouldInstantiateClassWithServiceArrayAsConstructorArgument()
        {
            var container = GetContainerWithMockSampleServices();
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
            var container = GetContainerWithMockSampleServices();
            container.AddService(typeof(SampleClassWithServiceEnumerableAsConstructorArgument),
                                 typeof(SampleClassWithServiceEnumerableAsConstructorArgument));

            var result = container.GetService(typeof(SampleClassWithServiceEnumerableAsConstructorArgument)) as SampleClassWithServiceEnumerableAsConstructorArgument;

            Assert.IsNotNull(result);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Services);
            Assert.IsTrue(result.Services.Count() > 0);
        }

        [Test]
        public void ShouldConvertTypeIntoPredicateThatChecksIfTypeExistsInContainerAsEnumerableSetOfServices()
        {
            ServiceContainer container = GetContainerWithMockSampleServices();

            var predicate = typeof(IEnumerable<ISampleService>)
                .ExistsAsEnumerableSetOfServices();

            Assert.IsTrue(predicate(container));
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
            var expectedConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new[] { typeof(ISampleService), typeof(ISampleService) });
            Assert.IsNotNull(expectedConstructor);

            ConstructorInfo result = resolver.ResolveFrom(typeof(SampleClassWithMultipleConstructors), container);
            Assert.AreSame(expectedConstructor, result);
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
            var expectedConstructor = typeof(SampleClassWithAdditionalArgument).GetConstructor(new Type[] {typeof(ISampleService), typeof(int)});
            Assert.IsNotNull(expectedConstructor);

            var result = resolver.ResolveFrom(typeof(SampleClassWithAdditionalArgument), container, 42);
            Assert.AreSame(expectedConstructor, result);
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

            var instance = container.AutoCreate(typeof(SampleClassWithAdditionalArgument), 42) as SampleClassWithAdditionalArgument;
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.Argument == 42);
        }

        [Test]
        public void ShouldConstructParametersFromContainer()
        {
            var targetConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new[] { typeof(ISampleService), 
                typeof(ISampleService) });

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
        public void ShouldInstantiateObjectWithConstructorAndArguments()
        {
            ConstructorInfo targetConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new[] { typeof(ISampleService), 
                typeof(ISampleService) });

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
        public void ShouldInstantiateClassWithNonServiceArguments()
        {
            var container = new ServiceContainer();
            container.AddDefaultServices();
            
            container.AddService(typeof (SampleClassWithNonServiceArgument), typeof (SampleClassWithNonServiceArgument));

            var text = "Hello, World!";
            string serviceName = null;
            var result = container.GetService<SampleClassWithNonServiceArgument>(serviceName, text);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value == text);
        }
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
            Assert.IsTrue(services.Count() == 10);

            foreach (var service in services)
            {
                Assert.AreSame(mockSampleService.Object, service);
            }
            return container;
        }
    }
}
