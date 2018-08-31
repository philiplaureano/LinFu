namespace SampleLibrary.IOC
{
    public class SampleClassWithNamedParameters
    {
        public SampleClassWithNamedParameters(ISampleService otherService)
        {
            ServiceInstance = otherService;
        }

        public ISampleService ServiceInstance { get; }
    }
}