using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithServiceEnumerableAsConstructorArgument
    {
        public SampleClassWithServiceEnumerableAsConstructorArgument(IEnumerable<ISampleService> services)
        {
            Services = services;
        }
        public IEnumerable<ISampleService> Services
        {
            get; private set;
        }
    }
}
