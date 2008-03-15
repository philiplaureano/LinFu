using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface ITypeSurrogate
    {
        bool CanSurrogate(Type serviceType);
        object ProvideSurrogate(Type serviceType);
    }
}
