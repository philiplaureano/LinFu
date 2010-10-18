using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithInjectionField : ISampleService
    {
        [Inject] public ISampleService SomeField;

        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}