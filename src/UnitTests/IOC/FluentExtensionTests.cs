using NUnit.Framework;
using SampleLibrary;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public partial class FluentExtensionTests
    {
        [Test]
        public void ServiceContainerShouldSupportInjectingNamedOncePerRequestServices()
        {
            TestOncePerRequest("MyService", inject => inject.Using<SampleClass>());
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingNamedOncePerThreadServicesUsingContainerLambdas()
        {
            TestOncePerThread("MyService", inject => inject.Using(container => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingNamedOncePerThreadServicesUsingLambdas()
        {
            TestOncePerThread("MyService", inject => inject.Using(() => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingNamedServicesOncePerThread()
        {
            TestOncePerThread("MyService", inject => inject.Using<SampleClass>());
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingNamedSingletons()
        {
            TestSingleton("MyService", inject => inject.Using<SampleClass>());
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingNamedSingletonsUsingContainerLambdas()
        {
            TestSingleton("MyService", inject => inject.Using(container => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingNamedSingletonsUsingLambdas()
        {
            TestSingleton("MyService", inject => inject.Using(() => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingOncePerRequestServicesUsingContainerLambdas()
        {
            TestOncePerRequest("MyService", inject => inject.Using(container => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingOncePerRequestServicesUsingLambdas()
        {
            TestOncePerRequest("MyService", inject => inject.Using(() => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingServicesOncePerRequest()
        {
            TestOncePerRequest(string.Empty, inject => inject.Using<SampleClass>());
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingServicesOncePerRequestUsingContainerLambdas()
        {
            TestOncePerRequest(string.Empty, inject => inject.Using(container => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingServicesOncePerRequestUsingLambdas()
        {
            TestOncePerRequest(string.Empty, inject => inject.Using(() => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingServicesOncePerThread()
        {
            TestOncePerThread(string.Empty, inject => inject.Using<SampleClass>());
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingServicesOncePerThreadUsingContainerLambdas()
        {
            TestOncePerThread(string.Empty, inject => inject.Using(container => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingServicesOncePerThreadUsingLambdas()
        {
            TestOncePerThread(string.Empty, inject => inject.Using(() => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingSingletons()
        {
            TestSingleton(string.Empty, inject => inject.Using<SampleClass>());
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingSingletonsUsingContainerLambdas()
        {
            TestSingleton(string.Empty, inject => inject.Using(container => new SampleClass()));
        }

        [Test]
        public void ServiceContainerShouldSupportInjectingSingletonsUsingLambdas()
        {
            TestSingleton(string.Empty, inject => inject.Using(() => new SampleClass()));
        }
    }
}