using System;
using System.Linq;
using System.Runtime.Serialization;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using Moq;
using Xunit;
using SampleLibrary;
using SampleLibrary.IOC;
using SampleLibrary.IOC.BugFixes;

namespace LinFu.UnitTests.IOC
{
    public class InversionOfControlTests
    {
        private static void VerifyInitializeCall(ServiceContainer container, Mock<IInitialize> mockService)
        {
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            container.AddService(mockService.Object);


            mockService.Expect(i => i.Initialize(container)).AtMostOnce();

            // The container should return the same instance
            var firstResult = container.GetService<IInitialize>();
            var secondResult = container.GetService<IInitialize>();
            Assert.Same(firstResult, secondResult);

            // The Initialize() method should only be called once
            mockService.Verify();
        }

        [Fact]
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
            Assert.NotSame(service, mockService.Object);

            // Execute the method and 'catch' the target instance once the method call is made
            service.DoSomething();

            var holder = container.GetService<ITargetHolder>("SampleAroundInvokeInterceptorClass");
            Assert.Same(holder.Target, mockService.Object);
        }

        [Fact]
        public void ContainerMustAllowInjectingCustomFactoriesForNamedOpenGenericTypeDefinitions()
        {
            var container = new ServiceContainer();
            var factory = new SampleOpenGenericFactory();
            var serviceName = "MyService";

            container.AddFactory(serviceName, typeof(ISampleGenericService<>), factory);

            // The container must report that it *can* create
            // the generic service type 
            Assert.True(container.Contains(serviceName, typeof(ISampleGenericService<int>)));

            var result = container.GetService<ISampleGenericService<int>>(serviceName);

            Assert.NotNull(result);
            Assert.True(result.GetType() == typeof(SampleGenericImplementation<int>));
        }

        [Fact]
        public void ContainerMustAllowInjectingCustomFactoriesForOpenGenericTypeDefinitions()
        {
            var container = new ServiceContainer();
            var factory = new SampleOpenGenericFactory();

            container.AddFactory(typeof(ISampleGenericService<>), factory);

            // The container must report that it *can* create
            // the generic service type 
            Assert.True(container.Contains(typeof(ISampleGenericService<int>)));

            var result = container.GetService<ISampleGenericService<int>>();

            Assert.NotNull(result);
            Assert.True(result.GetType() == typeof(SampleGenericImplementation<int>));
        }

        [Fact]
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
            Assert.NotSame(mockInstance, result);

            var proxy = (IProxy)result;
            Assert.NotNull(proxy.Interceptor);
        }

        [Fact]
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
            Assert.NotSame(mockInstance, result);

            var proxy = (IProxy)result;
            Assert.NotNull(proxy.Interceptor);
        }

        [Fact]
        public void ContainerMustAllowSurrogatesForNonExistentServiceInstances()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<ISampleService>();
            var surrogate = mockService.Object;
            container.Inject<ISampleService>().Using((f, arguments) => surrogate).OncePerRequest();

            var result = container.GetService<ISampleService>();
            Assert.NotNull(result);
            Assert.Same(surrogate, result);
        }

        [Fact]
        public void ContainerMustAllowUntypedOpenGenericTypeRegistration()
        {
            var serviceType = typeof(ISampleGenericService<>);
            var implementingType = typeof(SampleGenericImplementation<>);

            var container = new ServiceContainer();
            container.AddService(serviceType, implementingType);

            var result = container.GetService<ISampleGenericService<long>>();
            Assert.NotNull(result);
        }

        [Fact]
        public void ContainerMustAllowUntypedServiceRegistration()
        {
            var container = new ServiceContainer();
            container.AddService(typeof(ISampleService), typeof(SampleClass));

            var service = container.GetService<ISampleService>();
            Assert.NotNull(service);
        }

        [Fact]
        public void ContainerMustBeAbleToAddExistingServiceInstances()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<ISerializable>();
            container.AddService(mockService.Object);

            var result = container.GetService<ISerializable>();
            Assert.Same(result, mockService.Object);
        }

        [Fact]
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
            Assert.True(services.Count() == 10);

            // The resulting set of services
            // must match the given service instance
            foreach (var service in services) Assert.Same(mockSampleService.Object, service);
        }

        [Fact]
        public void ContainerMustBeAbleToSuppressNamedServiceNotFoundErrors()
        {
            ExpectException<NamedServiceNotFoundException>(() =>
            {
                var container = new ServiceContainer();
                var instance = container.GetService("MyService", typeof(ISerializable));
                Assert.Null(instance);
            });
        }

        [Fact]
        public void ContainerMustBeAbleToSupressServiceNotFoundErrors()
        {
            var container = new ServiceContainer();
            container.SuppressErrors = true;

            var instance = container.GetService(typeof(ISerializable));
            Assert.Null(instance);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void ContainerMustGetMultipleServicesOfTheSameTypeInOneCall()
        {
            var container = new ServiceContainer();
            var mockServiceCount = 10;

            // Add a set of dummy services
            for (var i = 0; i < mockServiceCount; i++)
            {
                var mockService = new Mock<ISampleService>();
                container.AddService(string.Format("Service{0}", i + 1), mockService.Object);
            }

            var instances = container.GetServices<ISampleService>();
            foreach (var serviceInstance in instances)
            {
                Assert.True(typeof(ISampleService).IsAssignableFrom(serviceInstance?.GetType()));
                Assert.NotNull(serviceInstance);
            }
        }

        [Fact]
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
                Assert.True(ex?.GetType() != typeof(StackOverflowException));
            }
        }

        [Fact]
        public void ContainerMustHoldAnonymousFactoryInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            // Give it a random service interface type
            var serviceType = typeof(IDisposable);

            // Manually add the factory instance
            container.AddFactory(serviceType, mockFactory.Object);
            Assert.True(container.Contains(serviceType),
                $"The container needs to have a factory for service type '{serviceType}'");
        }

        [Fact]
        public void ContainerMustHoldNamedFactoryInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            // Randomly assign an interface type
            // NOTE: The actual interface type doesn't matter
            var serviceType = typeof(ISerializable);

            container.AddFactory("MyService", serviceType, mockFactory.Object);
            Assert.True(container.Contains("MyService", serviceType),
                "The container is supposed to contain a service named 'MyService'");

            var instance = new object();
            mockFactory.Expect(f => f.CreateInstance(
                    It.Is<IFactoryRequest>(
                        request => request.ServiceName == "MyService" && request.ServiceType == serviceType)))
                .Returns(instance);

            Assert.Same(instance, container.GetService("MyService", serviceType));
        }

        [Fact]
        public void ContainerMustInjectFactoryInstances()
        {
            var mockFactory = new Mock<IFactory<ISampleService>>();
            mockFactory.Expect(f => f.CreateInstance(It.IsAny<IFactoryRequest>())).Returns(new SampleClass());

            var container = new ServiceContainer();
            container.AddFactory(mockFactory.Object);

            var instance =
                (SampleClassWithFactoryDependency)container.AutoCreate(typeof(SampleClassWithFactoryDependency));

            Assert.NotNull(instance);

            var factory = instance.Factory;
            factory.CreateInstance(null);

            mockFactory.VerifyAll();
        }

        [Fact]
        public void ContainerMustListAvailableNamedServices()
        {
            var container = new ServiceContainer();
            container.AddService<ISampleService>("MyService", new SampleClass());

            var availableServices = container.AvailableServices;
            Assert.True(availableServices.Count() > 0);

            // There should be a matching service type
            // at this point
            var matches = from s in availableServices
                          where
                              s.ServiceType == typeof(ISampleService) &&
                              s.ServiceName == "MyService"
                          select s;

            Assert.True(matches.Count() > 0);
        }

        [Fact]
        public void ContainerMustListAvailableUnnamedServices()
        {
            var container = new ServiceContainer();
            container.AddService<ISampleService>(new SampleClass());

            var availableServices = container.AvailableServices;
            Assert.True(availableServices.Count() > 0);

            // There should be a matching service type
            // at this point
            var matches = from s in availableServices
                          where s.ServiceType == typeof(ISampleService)
                          select s;

            Assert.True(matches.Count() > 0);
        }

        [Fact]
        public void
            ContainerMustLoadAnyGenericServiceTypeInstanceFromAGenericConcreteClassMarkedWithTheImplementsAttribute()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            var serviceName = "NonSpecificGenericService";

            // The container must be able to create any type that derives from ISampleService<T>
            // despite whether or not the specific generic service type is explicitly registered as a service
            Assert.True(container.Contains(serviceName, typeof(ISampleGenericService<int>)));
            Assert.True(container.Contains(serviceName, typeof(ISampleGenericService<double>)));
            Assert.True(container.Contains(serviceName, typeof(ISampleGenericService<string>)));

            // Both service types must be valid services
            Assert.NotNull(container.GetService<ISampleGenericService<int>>());
            Assert.NotNull(container.GetService<ISampleGenericService<double>>());
            Assert.NotNull(container.GetService<ISampleGenericService<string>>());
        }

        [Fact]
        public void ContainerMustLoadAssemblyFromMemory()
        {
            var container = new ServiceContainer();
            container.LoadFrom(typeof(SampleClass).Assembly);

            // Verify that the container loaded the sample assembly into memory
            Assert.True(container.Contains(typeof(ISampleService)));
        }

        [Fact]
        public void
            ContainerMustLoadSpecificGenericServiceTypesFromAGenericConcreteClassMarkedWithTheImplementsAttribute()
        {
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            var serviceName = "SpecificGenericService";

            // The container must be able to create both registered service types
            Assert.True(container.Contains(serviceName, typeof(ISampleGenericService<int>)));
            Assert.True(container.Contains(serviceName, typeof(ISampleGenericService<double>)));

            // Both service types must be valid services
            Assert.NotNull(container.GetService<ISampleGenericService<int>>());
            Assert.NotNull(container.GetService<ISampleGenericService<double>>());
        }

        [Fact]
        public void ContainerMustReturnServiceInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            var serviceType = typeof(ISerializable);
            var instance = new object();

            container.AddFactory(serviceType, mockFactory.Object);

            // The container must call the IFactory.CreateInstance method
            mockFactory.Expect(
                f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.ServiceType == serviceType
                                                                        && request.Container == container))).Returns(
                instance);

            var result = container.GetService(serviceType);
            Assert.NotNull(result);
            Assert.Same(instance, result);

            mockFactory.VerifyAll();
        }

        [Fact]
        public void ContainerMustSupportGenericAddFactoryMethod()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();

            container.AddFactory(mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.Container == container)))
                .Returns(mockService.Object);

            Assert.NotNull(container.GetService<ISerializable>());
        }

        [Fact]
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
                                                  request.ServiceType == typeof(ISerializable)))).Returns(
                mockService.Object);

            // Test the syntax
            var result = container.GetService<ISerializable>();
            Assert.Same(mockService.Object, result);

            result = container.GetService<ISerializable>("MyService");
            Assert.Same(mockService.Object, result);
        }


        [Fact]
        public void ContainerMustSupportNamedGenericAddFactoryMethod()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();

            container.AddFactory("MyService", mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.Container == container)))
                .Returns(mockService.Object);

            Assert.NotNull(container.GetService<ISerializable>("MyService"));
        }

        [Fact]
        public void ContainerMustThrowErrorIfServiceNotFound()
        {
            ExpectException<ServiceNotFoundException>(() =>
            {
                var container = new ServiceContainer();
                var instance = container.GetService(typeof(ISerializable));
                Assert.Null(instance);
            });
        }

        [Fact]
        public void ContainerMustUseUnnamedAddFactoryMethodIfNameIsNull()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();

            var container = new ServiceContainer();

            var serviceType = typeof(ISerializable);

            // Add the service using a null name;
            // the container should register this factory
            // as if it had no name
            container.AddFactory(null, serviceType, mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(
                It.Is<IFactoryRequest>(request => request.Container == container &&
                                                  request.ServiceType == serviceType))).Returns(mockService.Object);

            // Verify the result
            var result = container.GetService<ISerializable>();
            Assert.Same(mockService.Object, result);
        }

        [Fact]
        public void ContainerMustUseUnnamedContainsMethodIfNameIsNull()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();
            var container = new ServiceContainer();

            var serviceType = typeof(ISerializable);

            // Use unnamed AddFactory method
            container.AddFactory(serviceType, mockFactory.Object);

            // The container should use the
            // IContainer.Contains(Type) method instead of the
            // IContainer.Contains(string, Type) method if the
            // service name is blank
            Assert.True(container.Contains(null, typeof(ISerializable)));
        }

        [Fact]
        public void ContainerMustUseUnnamedGetServiceMethodIfNameIsNull()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();
            var container = new ServiceContainer();


            var serviceType = typeof(ISerializable);
            mockFactory.Expect(
                    f => f.CreateInstance(It.Is<IFactoryRequest>(request => request.ServiceType == serviceType)))
                .Returns(mockService.Object);
            container.AddFactory(serviceType, mockFactory.Object);

            var result = container.GetService(null, serviceType);

            Assert.Same(mockService.Object, result);
        }

//        [Fact]
//        [Ignore("TODO: Implement this")]
//        public void ContainerServicesShouldBeLazyIfProxyFactoryExists()
//        {
//            var container = new ServiceContainer();
//            container.AddService<IProxyFactory>(new ProxyFactory());
//            Assert.True(container.Contains(typeof(IProxyFactory)));
//
//            // The instance should never be created
//            container.AddService(typeof(ISampleService), typeof(SampleLazyService));
//
//            var result = container.GetService<ISampleService>();
//            Assert.False(SampleLazyService.IsInitialized);
            
//        }

        [Fact]
        public void
            ContainerShouldBeAbleToCreateAGenericServiceImplementationThatHasAConstructorWithPrimitiveArguments()
        {
            var container = new ServiceContainer();
            container.LoadFrom(typeof(SampleService<>).Assembly);

            ISampleService<int> s = null;
            // All fail with ServiceNotFoundException.
            s = container.GetService<ISampleService<int>>(42, "frobozz", false);
            Assert.NotNull(s);

            s = container.GetService<ISampleService<int>>(42, "frobozz");
            Assert.NotNull(s);
            s = container.GetService<ISampleService<int>>(42, false);
            Assert.NotNull(s);

            s = container.GetService<ISampleService<int>>(null, "frobozz", false);
            Assert.NotNull(s);
        }

        [Fact]
        public void ContainerShouldBeAbleToRegisterGenericTypeAndResolveConcreteServiceType()
        {
            var container = new ServiceContainer();
            container.AddService(typeof(ISampleGenericService<>),
                typeof(SampleGenericClassWithOpenGenericImplementation<>));

            var instance = container.GetService<ISampleGenericService<int>>();
            Assert.NotNull(instance);
        }

        [Fact]
        public void
            ContainerShouldBeAbleToRegisterGenericTypeAndResolveConcreteServiceTypeUsingTheNonGenericGetServiceMethod()
        {
            var container = new ServiceContainer();
            container.AddService(typeof(ISampleGenericService<>),
                typeof(SampleGenericClassWithOpenGenericImplementation<>));

            var instance = container.GetService(typeof(ISampleGenericService<int>));
            Assert.NotNull(instance);
        }

        [Fact]
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
            Assert.NotNull(result);

            mockPreprocessor.VerifyAll();
        }

        [Fact]
        public void InitializerShouldOnlyBeCalledOncePerLifetime()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<IInitialize>();

            VerifyInitializeCall(container, mockService);
            VerifyInitializeCall(container, new Mock<IInitialize>());
        }

        [Fact]
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
            Assert.NotSame(service, mockService.Object);
            // Execute the method and 'catch' the target instance once the method call is made
            service.DoSomething();

            var holder = container.GetService<ITargetHolder>("SampleInterceptorClass");
            Assert.Same(holder.Target, mockService.Object);
        }

        [Fact]
        public void ShouldNotReturnNamedServicesForGetServiceCallsForAnonymousServices()
        {
            ExpectException<NamedServiceNotFoundException>(() =>
           {
               var container = new ServiceContainer();
               var myService = new MyService();
               container.AddService<IMyService>(myService);

               Assert.NotNull(container.GetService<IMyService>());

               Assert.Null(container.GetService<IMyService>("frobozz"));
           });
        }

        private void ExpectException<TException>(Action testToRun)
            where TException : Exception
        {
            try
            {
                testToRun();
            }
            catch (TException)
            {
                return;
            }

            Assert.False(true, $"Expected exception type '{typeof(TException).FullName}' not thrown");
        }
    }
}