using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Implements(typeof(ISampleGenericService<>), ServiceName = "NonSpecificGenericService")]
    public class SampleGenericClassWithOpenGenericImplementation<T> : ISampleGenericService<T>
    {
        public bool Called => throw new NotImplementedException();
    }
}