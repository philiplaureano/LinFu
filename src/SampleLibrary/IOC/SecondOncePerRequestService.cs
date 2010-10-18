using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Implements(typeof (ISampleService), LifecycleType.OncePerRequest, ServiceName = "SecondOncePerRequestService")]
    public class SecondOncePerRequestService : FirstOncePerRequestService
    {
    }
}