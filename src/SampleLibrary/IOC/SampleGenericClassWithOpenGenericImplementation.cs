using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Implements(typeof(ISampleGenericService<>), ServiceName = "NonSpecificGenericService")]
    public class SampleGenericClassWithOpenGenericImplementation<T> : ISampleGenericService<T>
    {
        public bool Called
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
