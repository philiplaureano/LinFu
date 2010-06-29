using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    public class SampleClassWithFactoryDependency
    {
        private readonly IFactory<ISampleService> _factory;

        public SampleClassWithFactoryDependency(IFactory<ISampleService> factory)
        {
            _factory = factory;
        }

        public IFactory<ISampleService> Factory
        {
            get { return _factory; }
        }
    }
}
