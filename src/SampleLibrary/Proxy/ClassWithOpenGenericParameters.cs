using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.Proxy
{
    public class ClassWithOpenGenericParameters
    {
        public virtual void DoSomething<T>(List<T> someList)
        {
        }
    }
}
