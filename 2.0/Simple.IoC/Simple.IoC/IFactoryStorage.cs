using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    internal interface IFactoryStorage
    {
        bool Contains(string serviceName);
        void Store(string serviceName, object factory);
        object Retrieve(string serviceName);
    }
}
