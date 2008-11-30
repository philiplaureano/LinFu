using LinFu.IoC.Configuration;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [Implements(typeof(ISampleService), LifecycleType.OncePerThread)]
    public class OncePerThreadSampleService : ISampleService
    {
        public void DoSomething()
        {
            throw new System.NotImplementedException();
        }
    }
}