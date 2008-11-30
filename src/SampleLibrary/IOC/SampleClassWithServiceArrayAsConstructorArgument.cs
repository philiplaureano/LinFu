using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithServiceArrayAsConstructorArgument
    {
        private ISampleService[] _services;
        public SampleClassWithServiceArrayAsConstructorArgument(ISampleService[] services)
        {
            _services = services;
        }
        public ISampleService[] Services
        {
            get { return _services; }
        }
    }
}
