using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Loaders
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class ImplementsAttribute : Attribute 
    {
        private Type _serviceType;
        private LifecycleType _lifeCycleType;
        public string ServiceName;
        public ImplementsAttribute(Type serviceType, LifecycleType lifeCycleType)
        {
            _serviceType = serviceType;
            _lifeCycleType = lifeCycleType;
        }

        public Type ServiceType
        {
            get { return _serviceType; }
        }

        public LifecycleType LifeCycleType
        {
            get { return _lifeCycleType; }
        }
    }
}
