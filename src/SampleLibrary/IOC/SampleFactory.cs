using System;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof (ISampleService))]
    public class SampleFactory : IFactory
    {
        #region IFactory Members

        public object CreateInstance(IFactoryRequest request)
        {
            return new SampleClass();
        }

        #endregion
    }
}