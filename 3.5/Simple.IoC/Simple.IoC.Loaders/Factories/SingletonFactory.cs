using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Factories
{
    public class SingletonFactory<TService, TImplementor> : IFactory<TService>, IFactory
        where TService : class
        where TImplementor : class, TService, new()
    {
        private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        #region IFactory<T> Members

        public virtual TService CreateInstance(IContainer container)
        {
            return SingletonCache.CreateInstance<TImplementor>();
        }

        #endregion

        #region IFactory Members

        object IFactory.CreateInstance(IContainer container)
        {
            TService result = CreateInstance(container);
            return result;
        }

        #endregion
    }
}
