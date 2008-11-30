using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.Proxy
{
    public abstract class ClassWithVirtualByRefMethod
    {
        public abstract void ByRefMethod(ref int a);        
    }
}
