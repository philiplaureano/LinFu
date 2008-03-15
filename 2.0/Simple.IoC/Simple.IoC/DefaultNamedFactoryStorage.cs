using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    internal class DefaultNamedFactoryStorage : INamedFactoryStorage
    {
        private readonly Dictionary<Type, IFactoryStorage> _storage = new Dictionary<Type, IFactoryStorage>();
        #region INamedFactoryStorage Members

        public bool ContainsFactory<T>(string serviceName)
        {
            if (!_storage.ContainsKey(typeof(T)))
                return false;

            return _storage[typeof(T)].Contains(serviceName);
        }

        public IFactory<T> Retrieve<T>(string serviceName)
        {
            return (IFactory<T>)_storage[typeof(T)].Retrieve(serviceName);
        }

        public void Store<T>(string serviceName, IFactory<T> factory)
        {
            if (!_storage.ContainsKey(typeof(T)))
                _storage[typeof(T)] = new FactoryStorage();

            _storage[typeof(T)].Store(serviceName, factory);
        }

        #endregion
    }
}
