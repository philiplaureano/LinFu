using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public class ServiceNotFoundException : Exception 
    {
        private Type _serviceType;
        private string _serviceName;
        public ServiceNotFoundException(Type serviceType)
        {
            _serviceType = serviceType;
        }
        public ServiceNotFoundException(string serviceName, Type serviceType)
        {
            _serviceType = serviceType;
            _serviceName = serviceName;
        }

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_serviceName))
                    return string.Format("Service type '{0}' not found!", _serviceType.FullName);

                return string.Format("The service with type '{0}' and name of '{1}' not found!", 
                    _serviceType.FullName, _serviceName);
            }
        }
        public Type ServiceType
        {
            get { return _serviceType; }
        }
    }
}
