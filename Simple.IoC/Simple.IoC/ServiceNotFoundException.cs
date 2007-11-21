using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public class ServiceNotFoundException : Exception 
    {
        private Type _serviceType;
        public ServiceNotFoundException(Type serviceType)
        {
            _serviceType = serviceType;
        }
        public override string Message
        {
            get
            {
                return string.Format("Service type '{0}' not found!", _serviceType.FullName);
            }
        }
        public Type ServiceType
        {
            get { return _serviceType; }
        }
    }
}
