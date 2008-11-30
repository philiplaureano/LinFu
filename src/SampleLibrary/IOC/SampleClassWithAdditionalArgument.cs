using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithAdditionalArgument
    {
        private readonly int _value;
        
        // This is just a dummy constructor used to confuse
        // the resolver
        public SampleClassWithAdditionalArgument(ISampleService arg1)
        {

        }
        public SampleClassWithAdditionalArgument(ISampleService arg1, int arg2)
        {
            _value = arg2;
        }
        public int Argument
        {
            get { return _value; }
        }
    }
}
