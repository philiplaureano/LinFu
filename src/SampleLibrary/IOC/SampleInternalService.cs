using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Implements(typeof (ISampleService), ServiceName = "internal")]
    internal class SampleInternalService : ISampleService
    {
        public void DoSomething()
        {
            // Do nothing
        }
    }
}