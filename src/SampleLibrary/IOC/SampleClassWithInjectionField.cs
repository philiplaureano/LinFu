using System;
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