using Xunit;
using SampleLibrary;

namespace LinFu.UnitTests.IOC
{
    public partial class FluentExtensionTests
    {
        [Fact]
        public void ServiceContainerShouldSupportInjectingNamedOncePerRequestServices()
        {
            TestOncePerRequest("MyService", inject => inject.Using<SampleClass>());
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingNamedOncePerThreadServicesUsingContainerLambdas()
        {
            TestOncePerThread("MyService", inject => inject.Using(container => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingNamedOncePerThreadServicesUsingLambdas()
        {
            TestOncePerThread("MyService", inject => inject.Using(() => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingNamedServicesOncePerThread()
        {
            TestOncePerThread("MyService", inject => inject.Using<SampleClass>());
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingNamedSingletons()
        {
            TestSingleton("MyService", inject => inject.Using<SampleClass>());
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingNamedSingletonsUsingContainerLambdas()
        {
            TestSingleton("MyService", inject => inject.Using(container => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingNamedSingletonsUsingLambdas()
        {
            TestSingleton("MyService", inject => inject.Using(() => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingOncePerRequestServicesUsingContainerLambdas()
        {
            TestOncePerRequest("MyService", inject => inject.Using(container => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingOncePerRequestServicesUsingLambdas()
        {
            TestOncePerRequest("MyService", inject => inject.Using(() => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingServicesOncePerRequest()
        {
            TestOncePerRequest(string.Empty, inject => inject.Using<SampleClass>());
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingServicesOncePerRequestUsingContainerLambdas()
        {
            TestOncePerRequest(string.Empty, inject => inject.Using(container => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingServicesOncePerRequestUsingLambdas()
        {
            TestOncePerRequest(string.Empty, inject => inject.Using(() => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingServicesOncePerThread()
        {
            TestOncePerThread(string.Empty, inject => inject.Using<SampleClass>());
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingServicesOncePerThreadUsingContainerLambdas()
        {
            TestOncePerThread(string.Empty, inject => inject.Using(container => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingServicesOncePerThreadUsingLambdas()
        {
            TestOncePerThread(string.Empty, inject => inject.Using(() => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingSingletons()
        {
            TestSingleton(string.Empty, inject => inject.Using<SampleClass>());
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingSingletonsUsingContainerLambdas()
        {
            TestSingleton(string.Empty, inject => inject.Using(container => new SampleClass()));
        }

        [Fact]
        public void ServiceContainerShouldSupportInjectingSingletonsUsingLambdas()
        {
            TestSingleton(string.Empty, inject => inject.Using(() => new SampleClass()));
        }
    }
}