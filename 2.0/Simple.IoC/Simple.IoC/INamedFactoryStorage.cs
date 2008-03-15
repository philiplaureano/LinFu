using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface INamedFactoryStorage
    {
        bool ContainsFactory<T>(string serviceName);
        IFactory<T> Retrieve<T>(string serviceName);
        void Store<T>(string serviceName, IFactory<T> factory);
    }
}
