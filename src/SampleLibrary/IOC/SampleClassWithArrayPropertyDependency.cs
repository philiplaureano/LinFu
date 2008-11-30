using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithArrayPropertyDependency
    {
        [Inject]
        public ISampleService[] Property { get; set; }
    }
}
