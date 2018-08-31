using System.Collections.Generic;

namespace SampleLibrary.IOC
{
    public class SampleClassWithServiceEnumerableAsConstructorArgument
    {
        public SampleClassWithServiceEnumerableAsConstructorArgument(IEnumerable<ISampleService> services)
        {
            Services = services;
        }

        public IEnumerable<ISampleService> Services { get; }
    }
}