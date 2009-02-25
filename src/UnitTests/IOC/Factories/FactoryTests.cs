using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;
using Moq;
using NUnit.Framework;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Factories
{
    [TestFixture]
    public class FactoryTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            // Create a new mock service instance on each
            // factory method call
            createInstance = request => (new Mock<ISerializable>()).Object;
        }

        [TearDown]
        public void Term()
        {
            createInstance = null;
        }

        #endregion

        private Func<IFactoryRequest, ISerializable> createInstance;

        [Test]
        public void GenericFactoryAdapterShouldCallUntypedFactoryInstance()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();
            var adapter = new FactoryAdapter<ISerializable>(mockFactory.Object);

            // The adapter itself should call the container on creation
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.Container == container)))
                .Returns(mockService.Object);

            Assert.IsInstanceOfType(typeof(IFactory), adapter);

            var factoryRequest = new FactoryRequest()
                              {
                                  ServiceName = null,
                                  ServiceType = typeof(ISerializable),
                                  Container = container
                              };

            adapter.CreateInstance(factoryRequest);

            mockFactory.VerifyAll();
        }

        [Test]
        public void OncePerRequestFactoryShouldCreateUniqueInstances()
        {
            var factory = new OncePerRequestFactory<ISerializable>(createInstance);

            ISerializable first = factory.CreateInstance(null);
            ISerializable second = factory.CreateInstance(null);

            // Both instances must be unique
            Assert.AreNotSame(first, second);
            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
        }

        [Test]
        public void OncePerThreadFactoryShouldCreateTheSameInstanceFromWithinTheSameThread()
        {
            IFactory<ISerializable> localFactory = new OncePerThreadFactory<ISerializable>(createInstance);

            ISerializable first = localFactory.CreateInstance(null);
            ISerializable second = localFactory.CreateInstance(null);

            // The two instances should be the same
            // since they were created from the same thread
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [Test]
        public void OncePerThreadFactoryShouldCreateUniqueInstancesFromDifferentThreads()
        {
            IFactory<ISerializable> localFactory = new OncePerThreadFactory<ISerializable>(createInstance);
            var resultList = new List<ISerializable>();

            Action<IFactory<ISerializable>> doCreate = factory =>
                                                           {
                                                               ISerializable instance = factory.CreateInstance(null);
                                                               ISerializable otherInstance = factory.CreateInstance(null);

                                                               // The two instances 
                                                               // within the same thread must match
                                                               Assert.AreSame(instance, otherInstance);
                                                               lock (resultList)
                                                               {
                                                                   resultList.Add(instance);
                                                               }
                                                           };


            // Create the instance in another thread
            IAsyncResult asyncResult = doCreate.BeginInvoke(localFactory, null, null);
            ISerializable localInstance = localFactory.CreateInstance(null);

            // Wait for the previous thread
            // to finish executing
            doCreate.EndInvoke(asyncResult);

            Assert.IsTrue(resultList.Count > 0);

            // Collect the results from the other thread
            ISerializable instanceFromOtherThread = resultList[0];

            Assert.IsNotNull(localInstance);
            Assert.IsNotNull(instanceFromOtherThread);
            Assert.AreNotSame(localInstance, instanceFromOtherThread);
        }

        [Test]
        public void SingletonFactoryShouldCreateTheSameInstanceOnce()
        {
            var factory = new SingletonFactory<ISerializable>(createInstance);
            var container = new ServiceContainer();

            var request = new FactoryRequest()
                              {
                                  ServiceName = null,
                                  Arguments = new object[0],
                                  Container = container,
                                  ServiceType = typeof(ISerializable)
                              };

            ISerializable first = factory.CreateInstance(request);
            ISerializable second = factory.CreateInstance(request);

            // Both instances must be the same
            Assert.AreSame(first, second);
            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
        }

        [Test]
        public void ShouldBeAbleToInstantiateCustomFactoryWithServiceArgumentsInConstructor()
        {
            var mock = new Mock<ISampleService>();
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");
            
            container.AddService(mock.Object);
            var result = container.GetService<string>("SampleFactoryWithConstructorArguments");

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void ShouldLoadStronglyTypedFactoryFromLoadFromExtensionMethod()
        {
            var container = new ServiceContainer();
            container.LoadFrom(typeof(SampleClass).Assembly);

            var serviceInstance = container.GetService<ISampleService>("Test");
            Assert.IsNotNull(serviceInstance);
        }
    }
}