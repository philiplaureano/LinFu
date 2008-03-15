using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC
{
    internal class FactoryStorage : IFactoryStorage
    {
        private readonly Dictionary<string, object> _factories = new Dictionary<string, object>();
        #region IFactoryStorage Members

        public bool Contains(string serviceName)
        {
            return _factories.ContainsKey(serviceName);
        }
        public void Store(string serviceName, object factory)
        {
            _factories[serviceName] = factory;
        }

        public object Retrieve(string serviceName)
        {
            return _factories[serviceName];
        }

        #endregion
    }
}
