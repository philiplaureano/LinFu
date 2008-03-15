using System;
using System.Collections.Generic;
using System.Text;
using NMock2;
using NUnit.Framework;

namespace Simple.IoC.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        private Mockery mock;
        [SetUp]
        public void Init()
        {
            mock = new Mockery();
        }
        [TearDown]
        public void Term()
        {
            mock.VerifyAllExpectationsHaveBeenMet();
        }
        [Test]
        public void ContainerShouldStoreGenericFactoryInstance()
        {
            IFactory<ITestObject> factory = mock.NewMock<IFactory<ITestObject>>();
            ITestObject testObject = mock.NewMock<ITestObject>();

            // Add the factory instance
            IContainer container = new SimpleContainer();
            container.AddFactory(factory);

            // Once a query for that test instance is activated,
            // the container should call CreateInstance
            Expect.Once.On(factory).Method("CreateInstance").Will(Return.Value(testObject));
            ITestObject testResult = container.GetService<ITestObject>();
            Assert.AreSame(testObject, testResult);
        }
        [Test]
        public void ContainerShouldStoreFactoryInstance()
        {
            IFactory factory = mock.NewMock<IFactory>();
            ITestObject testObject = mock.NewMock<ITestObject>();

            // Add the factory instance
            IContainer container = new SimpleContainer();
            container.AddFactory(typeof (ITestObject), factory);

            // Once a query for that test instance is activated,
            // the container should call CreateInstance
            Expect.Once.On(factory).Method("CreateInstance").Will(Return.Value(testObject));
            ITestObject testResult = container.GetService<ITestObject>();
            Assert.AreSame(testObject, testResult);
        }
        [Test]
        public void ContainerShouldAddServiceInstance()
        {
            ITestObject testObject = mock.NewMock<ITestObject>();
            IContainer container = new SimpleContainer();

            container.AddService(testObject);
            Assert.AreSame(container.GetService<ITestObject>(), testObject);
        }
        [Test]
        public void ContainerShouldInitializeObject()
        {
            IFactory<ITestObject2> factory = mock.NewMock<IFactory<ITestObject2>>();
            ITestObject2 testObject = mock.NewMock<ITestObject2>();

            // Add the factory instance
            IContainer container = new SimpleContainer();
            container.AddFactory(factory);

            // Once the container instantiates the object, it should 
            // initialize it
            Expect.Once.On(testObject).Method("Initialize").With(container);

            // Once a query for that test instance is activated,
            // the container should call CreateInstance
            Expect.Once.On(factory).Method("CreateInstance").Will(Return.Value(testObject));
            ITestObject2 testResult = container.GetService<ITestObject2>();
            Assert.AreSame(testObject, testResult);
        }

        [Test]
        public void ContainerShouldCallInjectorInterface()
        {
            IFactory<ITestObject2> factory = mock.NewMock<IFactory<ITestObject2>>();
            ITestObject2 testObject = mock.NewMock<ITestObject2>();
            ITypeInjector injector = mock.NewMock<ITypeInjector>();
            
            IContainer container = new SimpleContainer();
            container.TypeInjectors.Add(injector);
            container.AddFactory(factory);

            Expect.Once.On(injector).Method("CanInject").WithAnyArguments().Will(Return.Value(true));
            Expect.Once.On(injector).Method("Inject").WithAnyArguments().Will(Return.Value(testObject));
            Expect.Once.On(testObject).Method("Initialize").With(container);

            container.AddService(injector);
            Expect.Once.On(factory).Method("CreateInstance").Will(Return.Value(testObject));
            ITestObject2 testResult = container.GetService<ITestObject2>();          
        }
        [Test]
        public void ContainerShouldCallPropertyInjectorInterface()
        {
            IContainer container = new SimpleContainer();
            IPropertyInjector injector = mock.NewMock<IPropertyInjector>();
            container.PropertyInjectors.Add(injector);

            ITestObject testObject = mock.NewMock<ITestObject>();
            container.AddService(testObject);

            Expect.Once.On(injector).Method("CanInject").With(testObject, container).Will(Return.Value(true));
            Expect.Once.On(injector).Method("InjectProperties").With(testObject, container);

            ITestObject other = container.GetService<ITestObject>();

            Assert.AreSame(other, testObject);
        }
    }
}
