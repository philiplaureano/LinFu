using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.Proxy
{
    public class ClassWithGenericMethod
    {
        public virtual void DoSomething<T>()
        {
            throw new NotImplementedException();
        }
    }
}
