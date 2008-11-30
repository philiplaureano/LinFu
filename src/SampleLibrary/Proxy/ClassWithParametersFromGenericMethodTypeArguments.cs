using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.Proxy
{
    public class ClassWithParametersFromGenericMethodTypeArguments
    {
        public virtual void DoSomething<T>(T first, T second)
        {            
        }
    }
}
