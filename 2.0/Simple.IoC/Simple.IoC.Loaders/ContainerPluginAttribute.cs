using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Loaders
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContainerPluginAttribute : Attribute
    {
    }
}
