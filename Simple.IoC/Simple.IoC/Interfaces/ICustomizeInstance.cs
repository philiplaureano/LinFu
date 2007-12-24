using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface ICustomizeInstance
    {
        bool CanCustomize(string serviceName, Type serviceType);
        void Customize(string serviceName, Type serviceType, object instance);
    }
}
