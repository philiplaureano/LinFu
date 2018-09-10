using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinFu.AOP.Cecil;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;
using LinFu.UnitTests.Tools;
using Xunit;
using SampleLibrary;
using SampleLibrary.Proxy;

namespace LinFu.UnitTests.Proxy
{
    public class ProxyFactoryTests : BaseTestFixture
    {
        private ServiceContainer container;
        private Loader loader;
        private string filename = string.Empty;

        protected override void Init()
        {
            loader = new Loader();
            container = new ServiceContainer();

            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            LoadAssemblyUsing(typeof(ProxyFactory));
            LoadAssemblyUsing(typeof(InvocationInfoEmitter));

            filename = string.Format("{0}.dll", Guid.NewGuid());

            // Add the PEVerifier to the proxy generation process
            container.AddService<IVerifier>(new PeVerifier(filename));
        }

        protected override void Term()
        {
            loader = null;
            container = null;

            try
            {
                File.Delete(filename);
            }
            catch
            {
                // Do nothing
            }
        }

        private void LoadAssemblyUsing(Type embeddedType)
        {
            var location = embeddedType.Assembly.Location;
            var directory = Path.GetDirectoryName(location);
            var assemblyFilename = Path.GetFileName(location);

            container.LoadFrom(directory, assemblyFilename);
        }

        private T CreateProxy<T>(Func<IInvocationInfo, object> implementation)
        {
            var factory = container.GetService<IProxyFactory>();

            var interceptor = new MockInterceptor(implementation);
            return factory.CreateProxy<T>(interceptor);
        }

        [Fact]
        public void ShouldAllowProxyToInheritFromMultipleInstancesOfTheSameGenericInterfaceType()
        {
            IInterceptor interceptor = new MockInterceptor(body => null);

            var interfaces = new[] {typeof(IList<int>), typeof(IList<double>), typeof(IList<object>)};
            var factory = container.GetService<IProxyFactory>();
            var proxy = factory.CreateProxy<object>(interceptor, interfaces);

            var proxyType = proxy.GetType();

            // The proxy must implement all of the given interfaces
            foreach (var currentType in interfaces) Assert.True(currentType.IsAssignableFrom(proxyType));
        }

        [Fact]
        public void ShouldCacheProxyTypes()
        {
            var factory = new ProxyFactory();
            var baseType = typeof(ISampleService);

            var proxyType = factory.CreateProxyType(baseType, new Type[0]);
            var runCount = 10;

            // All subsequent results must return the same proxy type
            for (var i = 0; i < runCount; i++)
            {
                var currentType = factory.CreateProxyType(baseType, new Type[0]);
                Assert.Equal(proxyType, currentType);
                Assert.Same(proxyType, currentType);
            }
        }

        [Fact]
        public void ShouldCallInterceptorInstance()
        {
            var factory = container.GetService<IProxyFactory>();
            var mockInterceptor = new MockInterceptor(i => null);

            // Create the proxy instance and then make the call
            var proxyInstance = (ITest) factory.CreateProxy(typeof(object), mockInterceptor, typeof(ITest));
            proxyInstance.Execute();

            // The interceptor must be called
            Assert.True(mockInterceptor.Called);
        }

        [Fact]
        public void ShouldCreateProxyWithVirtualSetterInitializedInCtor()
        {
            var factory = container.GetService<IProxyFactory>();

            // Assign the ref/out value for the int argument
            Func<IInvocationInfo, object> implementation = info =>
            {
                var methodName = info.TargetMethod.Name;

                if (methodName == "DoSomething")
                    info.Arguments[0] = 54321;

                if (methodName == "get_SomeProp")
                    return "blah";

                return null;
            };

            var interceptor = new MockInterceptor(implementation);
            var proxy = factory.CreateProxy<SampleClassWithPropertyInitializedInCtor>(interceptor);

            int value;
            proxy.DoSomething(out value);

            // The two given arguments should match
            Assert.Equal("blah", proxy.SomeProp);
            Assert.Equal(54321, value);
        }

        [Fact]
        public void ShouldHaveDefaultConstructor()
        {
            var factory = container.GetService<IProxyFactory>();
            var proxyType = factory.CreateProxyType(typeof(object), new Type[0]);
            Assert.NotNull(proxyType);

            var constructor = proxyType.GetConstructor(new Type[0]);
            Assert.True(constructor != null);

            var instance = constructor.Invoke(new object[0]);
            Assert.NotNull(instance);
        }

        [Fact]
        public void ShouldHaveDefaultProxyFactoryInstance()
        {
            var factory = container.GetService<IProxyFactory>();
            Assert.NotNull(factory);
            Assert.True(factory.GetType() == typeof(ProxyFactory));
        }

        [Fact]
        public void ShouldHaveSerializableAttribute()
        {
            var factory = new ProxyFactory();
            var proxyType = factory.CreateProxyType(typeof(ISampleService), new Type[0]);

            var customAttributes = proxyType.GetCustomAttributes(typeof(SerializableAttribute), false);
            Assert.True(customAttributes != null && customAttributes.Count() > 0);
        }

        [Fact]
        public void ShouldImplementGivenInterfaces()
        {
            var interfaces = new[] {typeof(ISampleService), typeof(ISampleGenericService<int>)};

            // Note: The interceptor will never be executed
            var interceptor = new MockInterceptor(info => { throw new NotImplementedException(); });
            var factory = container.GetService<IProxyFactory>();

            var proxy = factory.CreateProxy(typeof(object), interceptor, interfaces.ToArray());
            var proxyType = proxy.GetType();

            // Make sure that the generated proxy implements
            // all of the given interfaces
            foreach (var currentType in interfaces) Assert.True(currentType.IsAssignableFrom(proxyType));
        }

        [Fact]
        public void ShouldImplementIProxy()
        {
            var factory = container.GetService<IProxyFactory>();
            var proxyType = factory.CreateProxyType(typeof(object), new[] {typeof(ISampleService)});

            var instance = Activator.CreateInstance(proxyType);
            Assert.True(instance is IProxy);
            Assert.True(instance is ISampleService);
        }

        [Fact]
        public void ShouldReportTypeArgumentsUsedInGenericMethodCall()
        {
            var genericParameterType = typeof(int);
            var proxy = CreateProxy<ClassWithGenericMethod>(info =>
            {
                // The generic parameter type must match the given parameter type
                Assert.Contains(genericParameterType, info.TypeArguments);
                return null;
            });

            proxy.DoSomething<int>();
        }

        [Fact]
        public void ShouldSupportMethodCallsWithGenericParametersFromGenericMethodTypeArguments()
        {
            var genericParameterType = typeof(int);
            var proxy = CreateProxy<ClassWithParametersFromGenericMethodTypeArguments>(info =>
            {
                // Match the type argument
                Assert.Contains(genericParameterType, info.TypeArguments);
                Assert.Equal(1, info.Arguments[0]);
                Assert.Equal(1, info.Arguments[1]);

                return null;
            });

            proxy.DoSomething(1, 1);
        }

        [Fact]
        public void ShouldSupportMethodCallsWithGenericParametersFromHostGenericTypeArguments()
        {
            var proxy = CreateProxy<ClassWithParametersFromHostGenericTypeArguments<double, string>>(info =>
            {
                // Match the type arguments
                Assert.Equal(typeof(double), info.ParameterTypes[0]);
                Assert.Equal(typeof(string), info.ParameterTypes[1]);

                // Match the argument values
                Assert.Equal(1.0, info.Arguments[0]);
                Assert.Equal("Test", info.Arguments[1]);

                return null;
            });

            proxy.DoSomething(1.0, "Test");
        }

        [Fact]
        public void ShouldSupportMethodCallsWithGenericReturnValuesFromGenericMethodTypeArguments()
        {
            var dummyList = new List<int>();

            // The dummy list will be altered if the method body is called
            Func<IInvocationInfo, object> methodBody = info =>
            {
                var typeArguments = info.TypeArguments;

                // Match the type arguments
                Assert.Equal(typeof(int), typeArguments[0]);
                dummyList.Add(12345);
                return 12345;
            };

            var proxy = CreateProxy<ClassWithMethodReturnTypeFromGenericTypeArguments>(methodBody);
            proxy.DoSomething<int>();

            Assert.True(dummyList.Count > 0);
        }

        [Fact]
        public void ShouldSupportMethodCallsWithGenericReturnValuesFromHostGenericTypeArguments()
        {
            var proxy = CreateProxy<ClassWithMethodReturnValueFromTypeArgument<int>>(
                info =>
                {
                    // Make sure that the method return type 
                    // matches the given return type
                    Assert.True(info.ReturnType == typeof(int));
                    return 54321;
                });

            var result = proxy.DoSomething();

            Assert.Equal(54321, result);
        }

        [Fact]
        public void ShouldSupportMethodCallsWithOpenGenericParameters()
        {
            var dummyList = new List<int>();

            // The dummy list will be altered if the method body is called
            Func<IInvocationInfo, object> methodBody = info =>
            {
                var typeArguments = info.TypeArguments;

                // Match the type arguments
                Assert.Equal(typeof(int), typeArguments[0]);

                dummyList.Add(12345);

                return dummyList;
            };

            var proxy = CreateProxy<ClassWithOpenGenericParameters>(methodBody);
            proxy.DoSomething(dummyList);
            Assert.True(dummyList.Count > 0);
        }

        [Fact]
        public void ShouldSupportMethodCallWithNestedOpenGenericParameters()
        {
            var dummyList = new Dictionary<int, List<string>>();

            // The dummy list will be altered if the method body is called
            Func<IInvocationInfo, object> methodBody = info =>
            {
                var typeArguments = info.TypeArguments;

                // Match the type arguments

                //Assert.Equal(typeArguments[0], typeof(int));

                dummyList.Add(1, new List<string> {"SomeValue"});

                return dummyList[1];
            };

            var proxy = CreateProxy<ClassWithNestedOpenGenericParameters>(methodBody);
            proxy.DoSomething(dummyList);
            Assert.True(dummyList.Count > 0);
        }

        [Fact]
        public void ShouldSupportMethodsCallsWithGenericTypeDefinitionReturnType()
        {
            var dummyList = new List<int>();

            // The dummy list will be altered if the method body is called
            Func<IInvocationInfo, object> methodBody = info =>
            {
                var typeArguments = info.TypeArguments;

                // Match the type arguments

                Assert.Equal(typeof(int), typeArguments[0]);
                dummyList.Add(12345);
                return dummyList;
            };

            var proxy = CreateProxy<ClassWithGenericTypeDefinitionReturnType>(methodBody);
            proxy.DoSomething<int>();
            Assert.True(dummyList.Count > 0);
        }

        [Fact]
        public void ShouldSupportOutArguments()
        {
            var factory = container.GetService<IProxyFactory>();

            // Assign the ref/out value for the int argument
            Func<IInvocationInfo, object> implementation = info =>
            {
                info.Arguments[0] = 54321;
                return null;
            };

            var interceptor = new MockInterceptor(implementation);
            var proxy = factory.CreateProxy<ClassWithVirtualMethodWithOutParameter>(interceptor);

            int value;
            proxy.DoSomething(out value);

            // The two given arguments should match
            Assert.Equal(54321, value);
        }

        [Fact]
        public void ShouldSupportRefArguments()
        {
            var factory = container.GetService<IProxyFactory>();

            // Assign the ref/out value for the int argument
            Func<IInvocationInfo, object> implementation = info =>
            {
                info.Arguments[0] = 54321;
                return null;
            };

            var interceptor = new MockInterceptor(implementation);
            var proxy = factory.CreateProxy<ClassWithVirtualByRefMethod>(interceptor);

            var value = 0;
            proxy.ByRefMethod(ref value);

            // The two given arguments should match
            Assert.Equal(54321, value);
        }


        [Fact]
        public void ShouldSupportSerialization()
        {
            var dummyList = new List<int>();

            // The dummy list will be altered if the method body is called
            Func<IInvocationInfo, object> methodBody = info =>
            {
                var typeArguments = info.TypeArguments;

                // Match the type arguments
                Assert.Equal(typeof(int), typeArguments[0]);
                dummyList.Add(12345);
                return dummyList;
            };

            var proxy = CreateProxy<ClassWithGenericTypeDefinitionReturnType>(methodBody);
            proxy.DoSomething<int>();
            Assert.True(dummyList.Count > 0);
        }

        [Fact]
        public void ShouldSupportSubclassingFromGenericTypes()
        {
            var factory = container.GetService<IProxyFactory>();
            var actualList = new List<int>();

            Func<IInvocationInfo, object> implementation = info =>
            {
                IList<int> list = actualList;
                return info.Proceed(list);
            };
            var interceptor = new MockInterceptor(implementation);
            var proxy = factory.CreateProxy<IList<int>>(interceptor);

            // Any item added to the proxy list should be added to the 
            // actual list
            proxy.Add(12345);

            Assert.True(interceptor.Called);
            Assert.True(actualList.Count > 0);
            Assert.True(actualList[0] == 12345);
        }
    }
}