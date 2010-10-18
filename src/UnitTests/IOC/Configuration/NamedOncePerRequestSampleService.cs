using System;
using LinFu.IoC.Configuration;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [Implements(typeof (ISampleService), LifecycleType.OncePerRequest, ServiceName = "MyService")]
    public class NamedOncePerRequestSampleService : ISampleService
    {
        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}