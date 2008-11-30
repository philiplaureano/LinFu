using LinFu.IoC.Configuration;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [Implements(typeof(ISampleService), LifecycleType.OncePerThread, ServiceName = "MyService")]
    public class NamedOncePerThreadSampleService : ISampleService
    {
        public void DoSomething()
        {
            throw new System.NotImplementedException();
        }
    }
}