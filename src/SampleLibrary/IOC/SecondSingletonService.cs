using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Implements(typeof(ISampleService), LifecycleType.Singleton, ServiceName = "Second")]
    public class SecondSingletonService : ISampleService
    {
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
