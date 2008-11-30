using LinFu.IoC.Configuration;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [Implements(typeof(ISampleService), LifecycleType.Singleton, ServiceName = "MyService")]
    public class NamedSingletonSampleService : ISampleService
    {
        public void DoSomething()
        {
            throw new System.NotImplementedException();
        }
    }
}