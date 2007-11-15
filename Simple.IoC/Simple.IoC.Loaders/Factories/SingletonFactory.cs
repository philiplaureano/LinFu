using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Factories
{
    public class SingletonFactory<TService, TImplementor> : IFactory<TService>
        where TService : class
        where TImplementor : TService, new()
    {
        #region IFactory<T> Members

        public virtual TService CreateInstance(IContainer container)
        {
            return SingletonCreator.Instance;
        }

        #endregion

        private static class SingletonCreator
        {
            internal static readonly TService Instance = new TImplementor();
        }
    }
}
