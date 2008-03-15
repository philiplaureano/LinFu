using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Reflection;

namespace LinFu.DynamicProxy
{
    [Serializable]
    public class ProxyObjectReference : IObjectReference, ISerializable
    {
        private readonly Type _baseType;
        private readonly IProxy _proxy;
        protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
        {
            // Deserialize the base type using its assembly qualified name
            string qualifiedName = info.GetString("__baseType");
            _baseType = Type.GetType(qualifiedName, true, false);

            ProxyFactory factory = new ProxyFactory();
            Type proxyType = factory.CreateProxyType(_baseType);

            ConstructorInfo[] constructors = proxyType.GetConstructors();
            // Initialize the proxy with the deserialized data
            object[] args = new object[] { info, context };
            _proxy = (IProxy)Activator.CreateInstance(proxyType, args);
        }
        #region IObjectReference Members

        public object GetRealObject(StreamingContext context)
        {
            return _proxy;
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }
}
