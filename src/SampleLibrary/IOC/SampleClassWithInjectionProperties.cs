using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithInjectionProperties : ISampleService
    {
        [Inject]
        public ISampleService SomeProperty { get; set; }

        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}