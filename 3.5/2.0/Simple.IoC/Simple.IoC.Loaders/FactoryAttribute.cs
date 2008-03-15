using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class FactoryAttribute : Attribute
    {
        private Type _instanceType;
        public FactoryAttribute(Type instanceType)
        {
            _instanceType = instanceType;
        }

        public Type InstanceType
        {
            get { return _instanceType; }
        }
    }
}
