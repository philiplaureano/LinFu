using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Factories
{
    internal static class SingletonCache
    {
        private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        public static T CreateInstance<T>()
            where T : class, new()
        {
            lock (_singletons)
            {
                if (_singletons.ContainsKey(typeof(T)))
                    _singletons[typeof(T)] = new T();
            }

            return (T)_singletons[typeof(T)];
        }
    }
}
