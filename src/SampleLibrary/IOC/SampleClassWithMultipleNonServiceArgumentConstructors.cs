using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Implements(typeof(ISampleService), ServiceName = "ClassWithMultipleNonServiceArgumentConstructors")]
    public class SampleClassWithMultipleNonServiceArgumentConstructors : ISampleService
    {
        public SampleClassWithMultipleNonServiceArgumentConstructors(string arg1)
        {
            // Throw an exception if the incorrect constructor is selected
            throw new InvalidOperationException("Incorrect Constructor Selected");
        }
        public SampleClassWithMultipleNonServiceArgumentConstructors(string arg1, int arg2)
        {
            // Throw an exception if the incorrect constructor is selected
            throw new InvalidOperationException("Incorrect Constructor Selected");
        }
        public SampleClassWithMultipleNonServiceArgumentConstructors(string arg1, int arg2, SampleEnum arg3, decimal arg4, int arg5)
        {
        }
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
