using System;

namespace SampleLibrary.IOC
{
    public class SampleClassWithUnmarkedInjectionProperties : ISampleService
    {
        public ISampleService SomeProperty { get; set; }


        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}