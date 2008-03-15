using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    internal class InstanceFactory<T> : IFactory<T>
    {
        private T _instance;

        public InstanceFactory(T instance)
        {
            _instance = instance;
        }
        #region IFactory<T> Members

        public T CreateInstance(IContainer container)
        {
            return _instance;
        }

        #endregion
    }
}
