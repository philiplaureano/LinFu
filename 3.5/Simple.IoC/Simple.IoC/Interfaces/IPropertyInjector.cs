using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface IPropertyInjector
    {
        bool CanInject(object instance, IContainer sourceContainer);
        void InjectProperties(object instance, IContainer sourceContainer);
    }
}
