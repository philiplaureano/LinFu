using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.Proxy
{
    public class ClassWithGenericTypeDefinitionReturnType
    {
        public virtual List<T> DoSomething<T>()
        {
            return new List<T>();
        }
    }
}
