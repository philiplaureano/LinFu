using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Implements(typeof (ISampleService), LifecycleType.Singleton, ServiceName = "First")]
    public class FirstSingletonService : ISampleService
    {
        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}