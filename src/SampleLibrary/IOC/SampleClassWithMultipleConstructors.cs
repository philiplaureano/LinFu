using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithMultipleConstructors
    {
        public SampleClassWithMultipleConstructors()
        {
            // Do nothing
        }

        public SampleClassWithMultipleConstructors(ISampleService service)
        {
            FirstService = service;
        }

        public SampleClassWithMultipleConstructors(ISampleService firstService, ISampleService secondService)
        {
            FirstService = firstService;
            SecondService = secondService;
        }

        public SampleClassWithMultipleConstructors(ISampleService firstService, ISampleService secondService, ISampleGenericService<int> otherService)
        {
            // This is a dummy constructor that will be used
            // to attempt to confuse the fuzzy constructor search
        }

        public SampleClassWithMultipleConstructors(ISampleService firstService, ISampleService secondService, 
            ISampleGenericService<int> otherService, ISampleGenericService<string> someOtherService)
        {
            // This is a dummy constructor that will be used
            // to attempt to confuse the fuzzy constructor search
        }

        public ISampleService FirstService
        {
            get; private set;
        }

        public ISampleService SecondService
        {
            get;
            private set;
        }
    }
}
