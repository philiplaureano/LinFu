using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Implements(typeof (ISampleService), LifecycleType.OncePerRequest, ServiceName = "FirstOncePerRequestService")]
    public class FirstOncePerRequestService : ISampleService
    {
        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}