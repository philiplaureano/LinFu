using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Factory(typeof(string), ServiceName = "SampleFactoryWithConstructorArguments")]
    public class SampleFactoryWithConstructorArguments : IFactory
    {
        public ISampleService _sample;

        public SampleFactoryWithConstructorArguments(ISampleService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _sample = service;
        }

        public object CreateInstance(IFactoryRequest request)
        {
            return "42";
        }
    }
}
