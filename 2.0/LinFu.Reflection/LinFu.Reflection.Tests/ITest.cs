using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Reflection.Tests
{
    public interface ITest
    {
        void TargetMethod();
        void TargetMethod<T>();
        object TargetProperty { get; set;}
    }
}
