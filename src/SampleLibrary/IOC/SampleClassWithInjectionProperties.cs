using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithInjectionProperties : ISampleService 
    {
        [Inject] 
        public ISampleService SomeProperty { get; set; }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
