using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithInjectionProperties : ISampleService
    {
        [Inject] public ISampleService SomeProperty { get; set; }


        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}