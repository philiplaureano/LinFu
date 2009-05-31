using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.AOP
{
    public class SampleClassWithNewInstanceCall
    {
        public ISampleService DoSomething()
        {
            return new SampleServiceImplementation();
        }
    }
}
