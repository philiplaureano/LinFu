using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface ITypeInjector
    {
        bool CanInject(Type serviceType, object instance);
        object Inject(Type serviceType, object instance);
    }
}
