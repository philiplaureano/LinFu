using System.Collections.Generic;

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