using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using LinFu.AOP;
using LinFu.AOP.Cecil;
using LinFu.Proxy;
using LinFu.IoC.Reflection;
using LinFu.Reflection.Emit;
using LinFu.AOP.Interfaces;
using Moq;
using System.Reflection;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class ThirdPartyMethodCallInterceptionTests : BaseTestFixture
    {    
        [SetUp]
        public void Init()
        {            
        }

        [TearDown]
        public void Term()
        {
            AroundInvokeRegistry.Clear();
        }

        [Test]
        public void ShouldCallStaticAroundInvokeProvider()
        {
            Type modifiedTargetType = GetModifiedTargetType();
            var instance = Activator.CreateInstance(modifiedTargetType);            
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            AroundInvokeRegistry.AddProvider(provider);

            MethodInfo targetMethod = modifiedTargetType.GetMethod("DoSomething");

            targetMethod.Invoke(instance, null);

            Assert.IsTrue(aroundInvoke.BeforeInvokeWasCalled);
            Assert.IsTrue(aroundInvoke.AfterInvokeWasCalled);
        }

        [Test]
        public void ShouldCallInstanceAroundInvokeProvider()
        {
            Type modifiedTargetType = GetModifiedTargetType();
            var instance = Activator.CreateInstance(modifiedTargetType);
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            IModifiableType modified = (IModifiableType) instance;
            modified.AroundInvokeProvider = provider;
            MethodInfo targetMethod = modifiedTargetType.GetMethod("DoSomething");

            targetMethod.Invoke(instance, null);

            Assert.IsTrue(aroundInvoke.BeforeInvokeWasCalled);
            Assert.IsTrue(aroundInvoke.AfterInvokeWasCalled);
        }

        [Test]
        public void ShouldImplementIMethodReplacementHostOnTargetType()
        {
            Type modifiedTargetType = GetModifiedTargetType();

            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is IMethodReplacementHost);

            IMethodReplacementHost host = (IMethodReplacementHost)instance;

            var interceptor = new SampleMethodReplacement();

            host.MethodReplacementProvider = new SampleMethodReplacementProvider(interceptor);

            MethodInfo targetMethod = modifiedTargetType.GetMethod("DoSomething");
            try
            {
                targetMethod.Invoke(instance, null);
            }
            catch (TargetInvocationException ex)
            {
                var innerException = ex.InnerException;
                Console.WriteLine(ex.ToString());
                throw innerException;
            }

            Assert.IsTrue(interceptor.HasBeenCalled);
        }

        private Type GetModifiedTargetType()
        {
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");
            var module = assembly.MainModule;

            var typeName = "SampleClassWithThirdPartyMethodCall";
            var targetType = (from TypeDefinition t in module.Types
                              where t.Name == typeName
                              select t).First();

            // Intercept all calls to the System.Console.WriteLine method from the DoSomething method
            targetType.InterceptMethodCalls(t => t.Name.Contains(typeName), m => m.DeclaringType.Name.Contains(typeName) && m.Name == "DoSomething",
                                            methodCall => methodCall.DeclaringType.Name == "Console" && methodCall.Name == "WriteLine");

            var modifiedAssembly = assembly.ToAssembly();
            return (from t in modifiedAssembly.GetTypes()
                    where t.Name == typeName
                    select t).First();
        }
    }
}
