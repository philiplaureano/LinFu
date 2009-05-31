using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using LinFu.AOP;
using LinFu.AOP.Cecil;
using LinFu.AOP.Interfaces;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using NUnit.Framework;
using SampleLibrary.Proxy;
using SampleLibrary;
using System.Runtime.Serialization;
using System.Reflection;

namespace LinFu.UnitTests.Proxy
{
    [TestFixture]
    public class ProxySerializationTests : BaseTestFixture 
    {
        [Test]
        public void ShouldSupportSerialization()
        {
            var factory = new ProxyFactory();

            var interceptor = new SerializableInterceptor();
            interceptor.Identifier = Guid.NewGuid();

            var proxy = factory.CreateProxy<ISampleService>(interceptor);
            var proxyType = proxy.GetType();

            var proxyAssembly = proxyType.Assembly.Location;

            // The proxy type should have a default constructor
            // and a serialization constructor
            var constructorFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var constructors = proxyType.GetConstructors(constructorFlags);
            Assert.IsTrue(constructors.Length == 2);

            var serializationConstructor = proxyType.GetConstructor(constructorFlags, null,new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
            Assert.IsNotNull(serializationConstructor);

            // Serialize the proxy
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, proxy);

            // Deserialize the proxy from the stream
            stream.Seek(0, SeekOrigin.Begin);
            IProxy restoredProxy = (IProxy)formatter.Deserialize(stream);
            Assert.IsNotNull(restoredProxy);
            Assert.IsNotNull(restoredProxy.Interceptor);
            Assert.IsTrue(restoredProxy.Interceptor.GetType() == typeof(SerializableInterceptor));

            var otherInterceptor = (SerializableInterceptor)restoredProxy.Interceptor;
            Assert.AreEqual(otherInterceptor.Identifier, interceptor.Identifier);
        }
    }
}
