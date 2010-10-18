using System;

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