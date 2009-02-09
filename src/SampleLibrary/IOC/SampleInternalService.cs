using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Implements(typeof(ISampleService), ServiceName = "internal")]
    internal class SampleInternalService : ISampleService
    {
        public void DoSomething()
        {
            // Do nothing
        }
    }
}
