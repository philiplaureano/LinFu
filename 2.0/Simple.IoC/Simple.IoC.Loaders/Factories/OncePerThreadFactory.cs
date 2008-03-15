using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Simple.IoC.Factories
{
    public class OncePerThreadFactory<TService, TImplementor> : IFactory<TService>
        where TService : class
        where TImplementor : TService, new()
    {
        private static Dictionary<int, TService> _storage = new Dictionary<int, TService>();
        #region IFactory<T> Members

        public TService CreateInstance(IContainer container)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            TService result = null;
            lock(_storage)
            {
                if (!_storage.ContainsKey(threadId))
                    _storage[threadId] = new TImplementor();

                result = _storage[threadId];
            }

            return result;
        }

        #endregion
    }
}
