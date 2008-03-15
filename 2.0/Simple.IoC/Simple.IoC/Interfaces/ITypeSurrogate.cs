using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface ITypeSurrogate
    {
        bool CanSurrogate(string serviceName, Type serviceType);
        object ProvideSurrogate(string serviceName, Type serviceType);
    }
}
