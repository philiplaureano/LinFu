using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC.Extensions
{
    public class NamedCustomizer<T> : ICustomizeInstance
        where T : class
    {
        private string _serviceName;
        private Action<T> _customize;
        public NamedCustomizer(string serviceName, Action<T> customize)
        {
            _serviceName = serviceName;
            _customize = customize;
        }
        public bool CanCustomize(string serviceName, Type serviceType, IContainer hostContainer)
        {
            if (typeof(T) != serviceType)
                return false;

            if (serviceName != _serviceName)
                return false;

            if (_customize == null)
                return false;

            return true;
        }

        public void Customize(string serviceName, Type serviceType, object instance, IContainer hostContainer)
        {
            if (_serviceName != serviceName)
                return;

            if (serviceType != typeof(T))
                return;

            T target = instance as T;
            if (instance == null || target == null)
                return;

            _customize(target);
        }
    }
}
