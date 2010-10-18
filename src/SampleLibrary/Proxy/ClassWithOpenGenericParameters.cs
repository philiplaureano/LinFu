using System.Collections.Generic;

namespace SampleLibrary.Proxy
{
    public class ClassWithOpenGenericParameters
    {
        public virtual void DoSomething<T>(List<T> someList)
        {
        }
    }
}