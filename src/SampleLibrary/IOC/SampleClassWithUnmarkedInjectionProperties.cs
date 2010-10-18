using System;

namespace SampleLibrary.IOC
{
    public class SampleClassWithUnmarkedInjectionProperties : ISampleService
    {
        public ISampleService SomeProperty { get; set; }

        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}