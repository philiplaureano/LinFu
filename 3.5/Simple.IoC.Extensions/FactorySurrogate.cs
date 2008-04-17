using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC.Extensions
{
    public class FactorySurrogate<T> : ITypeSurrogate
        where T : class
    {
        private string _serviceName;
        private Func<T> _factoryMethod;
        private Action<T> _initialize;
        public FactorySurrogate(string serviceName, Func<T> factoryMethod, Action<T> initialize)
        {
            _serviceName = serviceName;
            _factoryMethod = factoryMethod;
            _initialize = initialize;
        }

        public bool CanSurrogate(string serviceName, Type serviceType)
        {
            if (_serviceName != serviceName)
                return false;

            if (serviceType != typeof(T))
                return false;

            return true;
        }

        public object ProvideSurrogate(string serviceName, Type serviceType)
        {
            if (_factoryMethod == null)
                throw new NotImplementedException();

            var result = _factoryMethod();

            // Initialize the item, if necessary
            if (_initialize != null)
                _initialize(result);

            return result;
        }
    }
}
