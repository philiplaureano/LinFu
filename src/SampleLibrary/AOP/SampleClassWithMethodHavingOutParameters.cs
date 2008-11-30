using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.AOP
{
    public class SampleClassWithMethodHavingOutParameters
    {
        public void DoSomething(out int a)
        {
            a = 12345;
        }
    }
}
