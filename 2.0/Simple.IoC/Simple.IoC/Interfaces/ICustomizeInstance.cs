using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface ICustomizeInstance
    {
        bool CanCustomize(string serviceName, Type serviceType, IContainer hostContainer);
        void Customize(string serviceName, Type serviceType, object instance, IContainer hostContainer);
    }
}
