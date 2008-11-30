using LinFu.IoC.Configuration;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [Implements(typeof(ISampleService), LifecycleType.OncePerRequest)]
    public class OncePerRequestSampleService : ISampleService
    {
        public void DoSomething()
        {
            throw new System.NotImplementedException();
        }
    }
}