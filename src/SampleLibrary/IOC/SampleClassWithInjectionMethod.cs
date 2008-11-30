using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithInjectionMethod : ISampleService 
    {
        public ISampleService Property
        {
            get; private set;
        }

        [Inject]
        public void DoSomething(ISampleService sample)
        {
            Property = sample;    
        }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
