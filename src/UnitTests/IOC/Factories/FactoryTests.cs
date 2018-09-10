using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LinFu.IoC;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;
using Moq;
using Xunit;
using SampleLibrary;
using SampleLibrary.IOC.BugFixes;

namespace LinFu.UnitTests.IOC.Factories
{
    public class FactoryTests : BaseTestFixture
    {
        private Func<IFactoryRequest, ISerializable> _createInstance;

        protected override void Init()
        {
            // Create a new mock service instance on each
            // factory method call
            _createInstance = request => new Mock<ISerializable>().Object;
        }

        protected override void Term()
        {
            _createInstance = null;
        }

        [Fact]
        public void GenericFactoryAdapterShouldCallUntypedFactoryInstance()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();
            var adapter = new FactoryAdapter<ISerializable>(mockFactory.Object);

            // The adapter itself should call the container on creation
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.Container == container)))
                .Returns(mockService.Object);

            Assert.NotNull(adapter);

            var factoryRequest = new FactoryRequest
            {
                ServiceName = null,
                ServiceType = typeof(ISerializable),
                Container = container
            };

            adapter.CreateInstance(factoryRequest);

            mockFactory.VerifyAll();
        }

        [Fact]
        public void OncePerRequestFactoryShouldCreateUniqueInstances()
        {
            var factory = new OncePerRequestFactory<ISerializable>(_createInstance);

            var first = factory.CreateInstance(null);
            var second = factory.CreateInstance(null);

            // Both instances must be unique
            Assert.NotSame(first, second);
            Assert.NotNull(first);
            Assert.NotNull(second);
        }

        [Fact]
        public void OncePerThreadFactoryShouldCreateTheSameInstanceFromWithinTheSameThread()
        {
            IFactory<ISerializable> localFactory = new OncePerThreadFactory<ISerializable>(_createInstance);

            var first = localFactory.CreateInstance(null);
            var second = localFactory.CreateInstance(null);

            // The two instances should be the same
            // since they were created from the same thread
            Assert.NotNull(first);
            Assert.Same(first, second);
        }

        [Fact]
        public void OncePerThreadFactoryShouldCreateUniqueInstancesFromDifferentThreads()
        {
            IFactory<ISerializable> localFactory = new OncePerThreadFactory<ISerializable>(_createInstance);
            var resultList = new List<ISerializable>();

            Action<IFactory<ISerializable>> doCreate = factory =>
            {
                var instance = factory.CreateInstance(null);
                var otherInstance = factory.CreateInstance(null);

                // The two instances 
                // within the same thread must match
                Assert.Same(instance, otherInstance);
                lock (resultList)
                {
                    resultList.Add(instance);
                }
            };


            // Create the instance in another thread
            var asyncResult = doCreate.BeginInvoke(localFactory, null, null);
            var localInstance = localFactory.CreateInstance(null);

            // Wait for the previous thread
            // to finish executing
            doCreate.EndInvoke(asyncResult);

            Assert.True(resultList.Count > 0);

            // Collect the results from the other thread
            var instanceFromOtherThread = resultList[0];

            Assert.NotNull(localInstance);
            Assert.NotNull(instanceFromOtherThread);
            Assert.NotSame(localInstance, instanceFromOtherThread);
        }

        [Fact]
        public void ShouldBeAbleToCreateClosedGenericTypeUsingACustomFactoryInstance()
        {
            var container = new ServiceContainer();
            container.Initialize();
            container.LoadFrom(typeof(MyClass<>).Assembly);

            // Get ServiceNotFoundException here instead of a service instance.
            var serviceName = "frobozz";
            var service = container.GetService<MyClass<string>>(serviceName);

            Console.WriteLine("foo");
            Assert.Equal(serviceName, service.Value);
        }

        [Fact]
        public void ShouldBeAbleToInstantiateCustomFactoryWithServiceArgumentsInConstructor()
        {
            var mock = new Mock<ISampleService>();
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");

            container.AddService(mock.Object);
            var result = container.GetService<string>("SampleFactoryWithConstructorArguments");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ShouldLoadStronglyTypedFactoryFromLoadFromExtensionMethod()
        {
            var container = new ServiceContainer();
            container.LoadFrom(typeof(SampleClass).Assembly);

            var serviceInstance = container.GetService<ISampleService>("Test");
            Assert.NotNull(serviceInstance);
        }

        [Fact]
        public void SingletonFactoryShouldCreateTheSameInstanceOnce()
        {
            var factory = new SingletonFactory<ISerializable>(_createInstance);
            var container = new ServiceContainer();

            var request = new FactoryRequest
            {
                ServiceName = null,
                Arguments = new object[0],
                Container = container,
                ServiceType = typeof(ISerializable)
            };

            var first = factory.CreateInstance(request);
            var second = factory.CreateInstance(request);

            // Both instances must be the same
            Assert.Same(first, second);
            Assert.NotNull(first);
            Assert.NotNull(second);
        }
    }
}