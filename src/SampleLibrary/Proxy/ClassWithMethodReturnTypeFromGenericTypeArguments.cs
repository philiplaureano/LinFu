using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.Proxy
{
    public class ClassWithMethodReturnTypeFromGenericTypeArguments
    {
        public virtual T DoSomething<T>()
        {
            return default(T);
        }
    }
}
