using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using Simple.IoC.Loaders;

namespace LinFu.MxClone
{
    [Implements(typeof(IInstanceHolder), LifecycleType.OncePerRequest)]
    public class DefaultInstanceHolder : IInstanceHolder
    {
        private readonly Dictionary<string, object> _instances = new Dictionary<string, object>();
        #region IInstanceHolder Members

        public void Register(string instanceName, object instance)
        {
            _instances[instanceName] = instance;
        }
        public object GetInstance(string instanceName)
        {
            return GetInstance<object>(instanceName);
        }
        public T GetInstance<T>(string instanceName)
            where T : class
        {
            if (!_instances.ContainsKey(instanceName))
                return null;

            return _instances[instanceName] as T;
        }

        #endregion
    }
}
