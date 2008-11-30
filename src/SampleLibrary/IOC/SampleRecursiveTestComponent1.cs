using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleRecursiveTestComponent1
    {
        public SampleRecursiveTestComponent1(SampleRecursiveTestComponent2 other)
        {            
        }
    }
}
