using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using LinFu.AOP.Cecil;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using Xunit;
using SampleLibrary;
using SampleLibrary.Proxy;

namespace LinFu.UnitTests.Proxy
{
    public class ProxySerializationTests : BaseTestFixture
    {
        [Fact]
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
            Assert.True(constructors.Length == 2);

            var serializationConstructor = proxyType.GetConstructor(constructorFlags, null,
                new[]
                {
                    typeof(SerializationInfo),
                    typeof(StreamingContext)
                }, null);
            
            Assert.NotNull(serializationConstructor);

            // Serialize the proxy
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, proxy);

            // Deserialize the proxy from the stream
            stream.Seek(0, SeekOrigin.Begin);
            var restoredProxy = (IProxy) formatter.Deserialize(stream);
            Assert.NotNull(restoredProxy);
            Assert.NotNull(restoredProxy.Interceptor);
            Assert.True(restoredProxy.Interceptor.GetType() == typeof(SerializableInterceptor));

            var otherInterceptor = (SerializableInterceptor) restoredProxy.Interceptor;
            Assert.Equal(otherInterceptor.Identifier, interceptor.Identifier);
        }
    }
}