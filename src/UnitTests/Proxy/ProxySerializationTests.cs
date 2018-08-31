using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using LinFu.AOP.Cecil;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.Proxy;

namespace LinFu.UnitTests.Proxy
{
    [TestFixture]
    public class ProxySerializationTests : BaseTestFixture
    {
        [Test]
        [Ignore]
        public void ShouldBeAbleToSerializeInvocationInfo()
        {
            var target = 42;
            var targetMethod = typeof(object).GetMethod("ToString");
            var stackTrace = new StackTrace();
            var parameterTypes = new[] {typeof(int)};
            var typeArguments = new[] {typeof(string)};
            var arguments = new object[] {1, 2, 3};
            var info = new InvocationInfo(target, targetMethod, stackTrace, parameterTypes, typeArguments,
                typeof(string), arguments);

            var memoryStream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, info);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var otherInfo = formatter.Deserialize(memoryStream) as InvocationInfo;
            Assert.IsNotNull(otherInfo);

            Assert.AreEqual(target, otherInfo.Target);
            Assert.AreEqual(targetMethod, otherInfo.TargetMethod);

            Assert.IsTrue(parameterTypes.AreEqualTo(otherInfo.ParameterTypes));
            Assert.IsTrue(typeArguments.AreEqualTo(otherInfo.TypeArguments));
            Assert.IsTrue(arguments.AreEqualTo(otherInfo.Arguments));
        }


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

            var serializationConstructor = proxyType.GetConstructor(constructorFlags, null,
                new[]
                {
                    typeof(SerializationInfo),
                    typeof(StreamingContext)
                }, null);
            Assert.IsNotNull(serializationConstructor);

            // Serialize the proxy
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, proxy);

            // Deserialize the proxy from the stream
            stream.Seek(0, SeekOrigin.Begin);
            var restoredProxy = (IProxy) formatter.Deserialize(stream);
            Assert.IsNotNull(restoredProxy);
            Assert.IsNotNull(restoredProxy.Interceptor);
            Assert.IsTrue(restoredProxy.Interceptor.GetType() == typeof(SerializableInterceptor));

            var otherInterceptor = (SerializableInterceptor) restoredProxy.Interceptor;
            Assert.AreEqual(otherInterceptor.Identifier, interceptor.Identifier);
        }
    }
}