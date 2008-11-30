using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithNonServiceArgument
    {
        private readonly string _value;
        public SampleClassWithNonServiceArgument(string value)
        {
            _value = value;
        }

        // This is just a dummy constructor that
        // will be used to confuse the constructor resolver
        public SampleClassWithNonServiceArgument(string arg1, string arg2)
        {

        }

        // This is just another dummy constructor that
        // will be used to confuse the constructor resolver
        public SampleClassWithNonServiceArgument(string arg1, string arg2, string arg3)
        {

        }

        public string Value
        {
            get { return _value; }
        }
    }
}
