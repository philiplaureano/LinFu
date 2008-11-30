using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithInjectionField : ISampleService
    {
        [Inject] public ISampleService SomeField;

        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
