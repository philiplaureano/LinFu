using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using Moq;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class InversionOfControlTests
    {
        [Test]
        public void ContainerShouldCallPreProcessor()
        {
            var mockPreprocessor = new Mock<IPreProcessor>();
            var mockService = new Mock<ISampleService>();

            mockPreprocessor.Expect(p => p.Preprocess(It.IsAny<IServiceRequest>()));

            var container = new ServiceContainer();
            container.AddService("SomeService", mockService.Object);
            container.PreProcessors.Add(mockPreprocessor.Object);

            // The preprocessors should be called
            var result = container.GetService<ISampleService>("SomeService");
            Assert.IsNotNull(result);

            mockPreprocessor.VerifyAll();
        }

        [Test]
        public void InterceptorClassesMarkedWithInterceptorAttributeMustGetActualTargetInstance()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var mockService = new Mock<ISampleInterceptedInterface>();
            mockService.Expect(mock => mock.DoSomething());

            // Add the target instance
            container.AddService(mockService.Object);

            // The service must return a proxy
            var service = container.GetService<ISampleInterceptedInterface>();
            Assert.AreNotSame(service, mockService.Object);

            // Execute the method and 'catch' the target instance once the method call is made
            service.DoSomething();

            var holder = container.GetService<ITargetHolder>("SampleInterceptorClass");
            Assert.AreSame(holder.Target, mockService.Object);
        }

        [Test]
        public void AroundInvokeClassesMarkedWithInterceptorAttributeMustGetActualTargetInstance()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var mockService = new Mock<ISampleWrappedInterface>();
            mockService.Expect(mock => mock.DoSomething());

            // Add the target instance
            container.AddService(mockService.Object);

            // The service must return a proxy
            var service = container.GetService<ISampleWrappedInterface>();
            Assert.AreNotSame(service, mockService.Object);

            // Execute the method and 'catch' the target instance once the method call is made
            service.DoSomething();

            var holder = container.GetService<ITargetHolder>("SampleAroundInvokeInterceptorClass");
            Assert.AreSame(holder.Target, mockService.Object);
        }

        [Test]
        public void ContainerMustAllowUntypedServiceRegistration()
        {
            var container = new ServiceContainer();
            container.AddService(typeof(ISampleService), typeof(SampleClass));

            var service = container.GetService<ISampleService>();
            Assert.IsNotNull(service);
        }

        [Test]
        public void ContainerMustAllowUntypedOpenGenericTypeRegistration()
        {
            var serviceType = typeof(ISampleGenericService<>);
            var implementingType = typeof(SampleGenericImplementation<>);

            var container = new ServiceContainer();
            container.AddService(serviceType, implementingType);

            var result = container.GetService<ISampleGenericService<long>>();
            Assert.IsNotNull(result);
        }

        [Test]
        public void ContainerMustAllowServicesToBeIntercepted()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            var mock = new Mock<ISampleInterceptedInterface>();
            var mockInstance = mock.Object;
            container.AddService(mockInstance);

            // The container must automatically load the interceptor
            // from the sample assembly
            var result = container.GetService<ISampleInterceptedInterface>();
            Assert.AreNotSame(mockInstance, result);

            var proxy = (IProxy)result;
            Assert.IsNotNull(proxy.Interceptor);
        }

        [Test]
        public void ContainerMustAllowServicesToBeInterceptedWithAnAroundInvokeInterceptor()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            var mock = new Mock<ISampleWrappedInterface>();
            var mockInstance = mock.Object;
            container.AddService(mockInstance);

            // The container must automatically load the IAroundInvoke
            // from the sample assembly
            var result = container.GetService<ISampleWrappedInterface>();
            Assert.AreNotSame(mockInstance, result);

            var proxy = (IProxy)result;
            Assert.IsNotNull(proxy.Interceptor);
        }
        [Test]
        public void ContainerMustAllowSurrogatesForNonExistentServiceInstances()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<ISampleService>();
            var surrogate = mockService.Object;
            container.Inject<ISampleService>().Using((f, arguments) => surrogate).OncePerRequest();

            var result = container.GetService<ISampleService>();
            Assert.IsNotNull(result);
            Assert.AreSame(surrogate, result);
        }

        [Test]
        public void InitializerShouldOnlyBeCalledOncePerLifetime()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<IInitialize>();

            VerifyInitializeCall(container, mockService);
            VerifyInitializeCall(container, new Mock<IInitialize>());
        }

        private static void VerifyInitializeCall(ServiceContainer container, Mock<IInitialize> mockService)
        {
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            container.AddService(mockService.Object);


            mockService.Expect(i => i.Initialize(container)).AtMostOnce();

            // The container should return the same instance
            var firstResult = container.GetService<IInitialize>();
            var secondResult = container.GetService<IInitialize>();
            Assert.AreSame(firstResult, secondResult);

            // The Initialize() method should only be called once
            mockService.Verify();
        }

        [Test]
        public void ContainerMustBeAbleToAddExistingServiceInstances()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<ISerializable>();
            container.AddService(mockService.Object);

            var result = container.GetService<ISerializable>();
            Assert.AreSame(result, mockService.Object);
        }

        [Test]
        [ExpectedException(typeof(NamedServiceNotFoundException))]
        public void ContainerMustBeAbleToSuppressNamedServiceNotFoundErrors()
        {
            var container = new ServiceContainer();
            object instance = container.GetService("MyService", typeof(ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
        }

        [Test]
        public void ContainerMustBeAbleToSupressServiceNotFoundErrors()
        {
            var container = new ServiceContainer();
            container.SuppressErrors = true;

            object instance = container.GetService(typeof(ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
        }

        [Test]
        public void ContainerMustBeAbleToReturnAListOfServices()
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

            // The resulting set of services
            // must match the given service instance
            foreach (var service in services)
            {
                Assert.AreSame(mockSampleService.Object, service);
            }
        }
        [Test]
        public void ContainerMustCallPostProcessorDuringARequest()
        {
            var mockPostProcessor = new Mock<IPostProcessor>();
            var container = new ServiceContainer();
            container.PostProcessors.Add(mockPostProcessor.Object);

            mockPostProcessor.Expect(p =>
                                     p.PostProcess(It.Is<IServiceRequestResult>(result => result != null)));

            container.SuppressErrors = true;
            container.GetService<ISerializable>();

            mockPostProcessor.VerifyAll();
        }

        [Test]
        public void ContainerMustHoldAnonymousFactoryInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            // Give it a random service interface type
            Type serviceType = typeof(IDisposable);

            // Manually add the factory instance
            container.AddFactory(serviceType, mockFactory.Object);
            Assert.IsTrue(container.Contains(serviceType), "The container needs to have a factory for service type '{0}'", serviceType);
        }

        [Test]
        public void ContainerMustHoldNamedFactoryInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            // Randomly assign an interface type
            // NOTE: The actual interface type doesn't matter
            Type serviceType = typeof(ISerializable);

            container.AddFactory("MyService", serviceType, mockFactory.Object);
            Assert.IsTrue(container.Contains("MyService", serviceType), "The container is supposed to contain a service named 'MyService'");

            var instance = new object();
            mockFactory.Expect(f => f.CreateInstance(
                It.Is<IFactoryRequest>(request => request.ServiceName == "MyService" && request.ServiceType == serviceType)))
                .Returns(instance);

            Assert.AreSame(instance, container.GetService("MyService", serviceType));
        }

        [Test]
        public void ContainerMustReturnServiceInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            Type serviceType = typeof(ISerializable);
            var instance = new object();

            container.AddFactory(serviceType, mockFactory.Object);

            // The container must call the IFactory.CreateInstance method
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.ServiceType == serviceType
                && request.Container == container))).Returns(instance);

            object result = container.GetService(serviceType);
            Assert.IsNotNull(result, "The container failed to return the given service instance");
            Assert.AreSame(instance, result, "The service instance returned does not match the given instance");

            mockFactory.VerifyAll();
        }

        [Test]
        public void ContainerMustSupportGenericAddFactoryMethod()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();

            container.AddFactory(mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.Container == container))).Returns(mockService.Object);

            Assert.IsNotNull(container.GetService<ISerializable>());
        }

        [Test]
        public void ContainerMustSupportGenericGetServiceMethod()
        {
            var mockService = new Mock<ISerializable>();
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            container.AddFactory(typeof(ISerializable), mockFactory.Object);
            container.AddFactory("MyService", typeof(ISerializable), mockFactory.Object);

            // Return the mock ISerializable instance
            mockFactory.Expect(f => f.CreateInstance(
                It.Is<IFactoryRequest>(request => request.Container == container &&
                    request.ServiceType == typeof(ISerializable)))).Returns(mockService.Object);

            // Test the syntax
            var result = container.GetService<ISerializable>();
            Assert.AreSame(mockService.Object, result);

            result = container.GetService<ISerializable>("MyService");
            Assert.AreSame(mockService.Object, result);
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void ContainerServicesShouldBeLazyIfProxyFactoryExists()
        {
            var container = new ServiceContainer();
            container.AddService<IProxyFactory>(new ProxyFactory());
            Assert.IsTrue(container.Contains(typeof(IProxyFactory)));

            // The instance should never be created
            container.AddService(typeof (ISampleService), typeof (SampleLazyService));

            var result = container.GetService<ISampleService>();
            Assert.IsFalse(SampleLazyService.IsInitialized);
        }


        [Test]
        public void ContainerMustSupportNamedGenericAddFactoryMethod()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();

            container.AddFactory("MyService", mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.Container == container)))
                .Returns(mockService.Object);

            Assert.IsNotNull(container.GetService<ISerializable>("MyService"));
        }

        [Test]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void ContainerMustThrowErrorIfServiceNotFound()
        {
            var container = new ServiceContainer();
            object instance = container.GetService(typeof(ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
        }

        [Test]
        public void ContainerMustUseUnnamedAddFactoryMethodIfNameIsNull()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();

            var container = new ServiceContainer();

            Type serviceType = typeof(ISerializable);

            // Add the service using a null name;
            // the container should register this factory
            // as if it had no name
            container.AddFactory(null, serviceType, mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(
                It.Is<IFactoryRequest>(request => request.Container == container &&
                    request.ServiceType == serviceType))).Returns(mockService.Object);

            // Verify the result
            var result = container.GetService<ISerializable>();
            Assert.AreSame(mockService.Object, result);
        }

        [Test]
        public void ContainerMustUseUnnamedContainsMethodIfNameIsNull()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();
            var container = new ServiceContainer();

            Type serviceType = typeof(ISerializable);

            // Use unnamed AddFactory method
            container.AddFactory(serviceType, mockFactory.Object);

            // The container should use the
            // IContainer.Contains(Type) method instead of the
            // IContainer.Contains(string, Type) method if the
            // service name is blank
            Assert.IsTrue(container.Contains(null, typeof(ISerializable)));
        }

        [Test]
        public void ContainerMustUseUnnamedGetServiceMethodIfNameIsNull()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();
            var container = new ServiceContainer();


            Type serviceType = typeof(ISerializable);
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.ServiceType == serviceType)))
                .Returns(mockService.Object);
            container.AddFactory(serviceType, mockFactory.Object);

            object result = container.GetService(null, serviceType);

            Assert.AreSame(mockService.Object, result);
        }
        [Test]
        public void ContainerMustAllowInjectingCustomFactoriesForOpenGenericTypeDefinitions()
        {
            var container = new ServiceContainer();
            var factory = new SampleOpenGenericFactory();

            container.AddFactory(typeof(ISampleGenericService<>), factory);

            // The container must report that it *can* create
            // the generic service type 
            Assert.IsTrue(container.Contains(typeof(ISampleGenericService<int>)));

            var result = container.GetService<ISampleGenericService<int>>();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(SampleGenericImplementation<int>));
        }

        [Test]
        public void ContainerMustListAvailableUnnamedServices()
        {
            var container = new ServiceContainer();
            container.AddService<ISampleService>(new SampleClass());

            var availableServices = container.AvailableServices;
            Assert.IsTrue(availableServices.Count() > 0);

            // There should be a matching service type
            // at this point
            var matches = from s in availableServices
                          where s.ServiceType == typeof(ISampleService)
                          select s;

            Assert.IsTrue(matches.Count() > 0);
        }
        [Test]
        public void ContainerMustListAvailableNamedServices()
        {
            var container = new ServiceContainer();
            container.AddService<ISampleService>("MyService", new SampleClass());

            var availableServices = container.AvailableServices;
            Assert.IsTrue(availableServices.Count() > 0);

            // There should be a matching service type
            // at this point
            var matches = from s in availableServices
                          where s.ServiceType == typeof(ISampleService) && s.ServiceName == "MyService"
                          select s;

            Assert.IsTrue(matches.Count() > 0);
        }
        [Test]
        public void ContainerMustAllowInjectingCustomFactoriesForNamedOpenGenericTypeDefinitions()
        {
            var container = new ServiceContainer();
            var factory = new SampleOpenGenericFactory();
            var serviceName = "MyService";

            container.AddFactory(serviceName, typeof(ISampleGenericService<>), factory);

            // The container must report that it *can* create
            // the generic service type 
            Assert.IsTrue(container.Contains(serviceName, typeof(ISampleGenericService<int>)));

            var result = container.GetService<ISampleGenericService<int>>(serviceName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(SampleGenericImplementation<int>));
        }

        [Test]
        public void ContainerMustCallIInitializeOnServicesCreatedFromCustomFactory()
        {
            var mockFactory = new Mock<IFactory>();
            var mockInitialize = new Mock<IInitialize>();

            mockFactory.Expect(f => f.CreateInstance(It.IsAny<IFactoryRequest>()))
                .Returns(mockInitialize.Object);

            // The IInitialize instance must be called once it
            // leaves the custom factory
            mockInitialize.Expect(i => i.Initialize(It.IsAny<IServiceContainer>()));

            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
            container.AddFactory(typeof(IInitialize), mockFactory.Object);

            var result = container.GetService<IInitialize>();

            mockFactory.VerifyAll();
            mockInitialize.VerifyAll();
        }

        [Test]
        public void ContainerMustGetMultipleServicesOfTheSameTypeInOneCall()
        {
            var container = new ServiceContainer();
            var mockServiceCount = 10;

            // Add a set of dummy services
            for (int i = 0; i < mockServiceCount; i++)
            {
                var mockService = new Mock<ISampleService>();
                container.AddService(string.Format("Service{0}", i + 1), mockService.Object);
            }

            var instances = container.GetServices<ISampleService>();
            foreach (var serviceInstance in instances)
            {
                Assert.IsInstanceOfType(typeof(ISampleService), serviceInstance);
                Assert.IsNotNull(serviceInstance);
            }
        }

        [Test]
        public void ContainerMustLoadSpecificGenericServiceTypesFromAGenericConcreteClassMarkedWithTheImplementsAttribute()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            var serviceName = "SpecificGenericService";

            // The container must be able to create both registered service types
            Assert.IsTrue(container.Contains(serviceName, typeof(ISampleGenericService<int>)));
            Assert.IsTrue(container.Contains(serviceName, typeof(ISampleGenericService<double>)));

            // Both service types must be valid services
            Assert.IsNotNull(container.GetService<ISampleGenericService<int>>());
            Assert.IsNotNull(container.GetService<ISampleGenericService<double>>());
        }

        [Test]
        public void ContainerMustLoadAnyGenericServiceTypeInstanceFromAGenericConcreteClassMarkedWithTheImplementsAttribute()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            var serviceName = "NonSpecificGenericService";

            // The container must be able to create any type that derives from ISampleService<T>
            // despite whether or not the specific generic service type is explicitly registered as a service
            Assert.IsTrue(container.Contains(serviceName, typeof(ISampleGenericService<int>)));
            Assert.IsTrue(container.Contains(serviceName, typeof(ISampleGenericService<double>)));
            Assert.IsTrue(container.Contains(serviceName, typeof(ISampleGenericService<string>)));

            // Both service types must be valid services
            Assert.IsNotNull(container.GetService<ISampleGenericService<int>>());
            Assert.IsNotNull(container.GetService<ISampleGenericService<double>>());
            Assert.IsNotNull(container.GetService<ISampleGenericService<string>>());
        }


        [Test]
        public void ContainerMustGracefullyHandleRecursiveServiceDependencies()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            container.AddService(typeof(SampleRecursiveTestComponent1), typeof(SampleRecursiveTestComponent1));
            container.AddService(typeof(SampleRecursiveTestComponent2), typeof(SampleRecursiveTestComponent2));

            try
            {
                var result = container.GetService<SampleRecursiveTestComponent1>();
            }
            catch (Exception ex)
            {
                Assert.IsNotInstanceOfType(typeof(StackOverflowException), ex);
            }
        }

        [Test]
        public void ContainerMustLoadAssemblyFromMemory()
        {
            var container = new ServiceContainer();
            container.LoadFrom(typeof (SampleClass).Assembly);
            
            // Verify that the container loaded the sample assembly into memory
            Assert.IsTrue(container.Contains(typeof (ISampleService)));
        }
    }
}
