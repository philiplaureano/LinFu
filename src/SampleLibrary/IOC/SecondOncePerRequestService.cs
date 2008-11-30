using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Implements(typeof(ISampleService), LifecycleType.OncePerRequest, ServiceName="SecondOncePerRequestService")]
    public class SecondOncePerRequestService : FirstOncePerRequestService
    {
    }
}
