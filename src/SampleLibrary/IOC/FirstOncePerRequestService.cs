using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Implements(typeof(ISampleService), LifecycleType.OncePerRequest, ServiceName = "FirstOncePerRequestService")]
    public class FirstOncePerRequestService : ISampleService
    {
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}