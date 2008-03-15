using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InitializerAttribute : Attribute
    {
    }
}
