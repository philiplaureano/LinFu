using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.Proxy
{
    public class ClassWithNestedOpenGenericParameters
    {
        public virtual List<T2> DoSomething<T1, T2>(Dictionary<T1, List<T2>> somelist) 
        {
            return null;
        }
    }
}
