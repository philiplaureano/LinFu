using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Implements(typeof (ISampleGenericService<>), ServiceName = "NonSpecificGenericService")]
    public class SampleGenericClassWithOpenGenericImplementation<T> : ISampleGenericService<T>
    {
        #region ISampleGenericService<T> Members

        public bool Called
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}