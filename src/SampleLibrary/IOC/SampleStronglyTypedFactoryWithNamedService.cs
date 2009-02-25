using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Factory(typeof(ISampleService), ServiceName="Test")]
    public class SampleStronglyTypedFactoryWithNamedService : IFactory<ISampleService>
    {
        #region IFactory<ISampleService> Members

        public ISampleService CreateInstance(IFactoryRequest request)
        {
            return new SampleClass();
        }

        #endregion
    }
}
