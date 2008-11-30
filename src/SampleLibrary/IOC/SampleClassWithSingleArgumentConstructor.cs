using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithSingleArgumentConstructor
    {
        public SampleClassWithSingleArgumentConstructor(ISampleService service)
        {
            Service = service;
        }
        public ISampleService Service
        {
            get; set;
        }
    }
}
