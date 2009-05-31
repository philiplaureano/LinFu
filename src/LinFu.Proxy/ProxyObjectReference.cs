using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using LinFu.Proxy.Interfaces;
using LinFu.AOP.Interfaces;
using System.Reflection;

namespace LinFu.Proxy
{
    /// <summary>
    /// Represents a helper class that deserializes proxy instances.
    /// </summary>
    [Serializable]
    public class ProxyObjectReference : IObjectReference, ISerializable
    {
        private readonly Type _baseType;
        private readonly IProxy _proxy;

        /// <summary>
        /// Initializes a new instance of the ProxyObjectReference class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> class that contains the serialized data.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that describes the serialization state.</param>
        protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
        {
            // Deserialize the base type using its assembly qualified name
            string qualifiedName = info.GetString("__baseType");
            _baseType = Type.GetType(qualifiedName, true, false);

            // Rebuild the list of interfaces
            List<Type> interfaceList = new List<Type>();
            int interfaceCount = info.GetInt32("__baseInterfaceCount");
            for (int i = 0; i < interfaceCount; i++)
            {
                string keyName = string.Format("__baseInterface{0}", i);
                string currentQualifiedName = info.GetString(keyName);
                Type interfaceType = Type.GetType(currentQualifiedName, true, false);

                interfaceList.Add(interfaceType);
            }

            // Reconstruct the proxy
            ProxyFactory factory = new ProxyFactory();
            Type proxyType = factory.CreateProxyType(_baseType, interfaceList.ToArray());
            _proxy = (IProxy)Activator.CreateInstance(proxyType);

            IInterceptor interceptor = (IInterceptor)info.GetValue("__interceptor", typeof(IInterceptor));
            _proxy.Interceptor = interceptor;
        }

        /// <summary>
        /// Returns the deserialized proxy instance.
        /// </summary>
        /// <param name="context">The <see cref="StreamingContext"/> that describes the serialization state.</param>
        /// <returns></returns>
        public object GetRealObject(StreamingContext context)
        {
            return _proxy;
        }

        /// <summary>
        /// Serializes the proxy to a stream. 
        /// </summary>
        /// <remarks>This method override does nothing.</remarks>
        /// <param name="info">The <see cref="SerializationInfo"/> class that contains the serialized data.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that describes the serialization state.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
