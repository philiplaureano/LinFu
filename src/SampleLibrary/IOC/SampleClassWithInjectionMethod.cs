using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithInjectionMethod : ISampleService
    {
        public ISampleService Property { get; private set; }

        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion

        [Inject]
        public void DoSomething(ISampleService sample)
        {
            Property = sample;
        }
    }
}