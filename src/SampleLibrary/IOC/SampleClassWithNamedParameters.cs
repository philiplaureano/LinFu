using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithNamedParameters
    {
        private readonly ISampleService _otherService;

        public SampleClassWithNamedParameters(ISampleService otherService)
        {
            _otherService = otherService;
        }

        public ISampleService ServiceInstance
        {
            get { return _otherService; }
        }
    }
}
