using System;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using NUnit.Framework;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class ThirdPartyMethodCallInterceptionTests : BaseTestFixture
    {
        [SetUp]
        public override void Init()
        {
        }

        [TearDown]
        public override void Term()
        {
            AroundInvokeMethodCallRegistry.Clear();
        }


        private Type GetModifiedTargetType()
        {
            return GetModifiedTargetType(Modify);
        }

        private Type GetModifiedTargetType(Action<string, TypeDefinition> modify)
        {
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");
            var module = assembly.MainModule;

            // Intercept all calls to the System.Console.WriteLine method from the DoSomething method
            var typeName = "SampleClassWithThirdPartyMethodCall";
            var targetType = (from TypeDefinition t in module.Types
                where t.Name == typeName
                select t).First();

            modify(typeName, targetType);

            var modifiedAssembly = assembly.ToAssembly();
            return (from t in modifiedAssembly.GetTypes()
                where t.Name == typeName
                select t).First();
        }

        private void Modify(string typeName, TypeDefinition targetType)
        {
            targetType.InterceptMethodCalls(t => t.Name.Contains(typeName),
                m => m.DeclaringType.Name.Contains(typeName) && m.Name == "DoSomething",
                methodCall =>
                    methodCall.DeclaringType.Name == "Console" && methodCall.Name == "WriteLine");
        }

        [Test]
        public void ShouldCallInstanceAroundInvokeProvider()
        {
            var modifiedTargetType = GetModifiedTargetType();
            var instance = Activator.CreateInstance(modifiedTargetType);
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            var modified = (IModifiableType) instance;
            modified.AroundMethodCallProvider = provider;
            var targetMethod = modifiedTargetType.GetMethod("DoSomething");

            targetMethod.Invoke(instance, null);

            Assert.IsTrue(aroundInvoke.BeforeInvokeWasCalled);
            Assert.IsTrue(aroundInvoke.AfterInvokeWasCalled);
        }

        [Test]
        public void ShouldCallStaticAroundInvokeProvider()
        {
            var modifiedTargetType = GetModifiedTargetType();
            var instance = Activator.CreateInstance(modifiedTargetType);
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            AroundInvokeMethodCallRegistry.AddProvider(provider);

            var targetMethod = modifiedTargetType.GetMethod("DoSomething");

            targetMethod.Invoke(instance, null);

            Assert.IsTrue(aroundInvoke.BeforeInvokeWasCalled);
            Assert.IsTrue(aroundInvoke.AfterInvokeWasCalled);
        }

        [Test]
        public void ShouldImplementIMethodReplacementHostOnTargetType()
        {
            var modifiedTargetType = GetModifiedTargetType();

            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is IMethodReplacementHost);

            var host = (IMethodReplacementHost) instance;

            var interceptor = new SampleMethodReplacement();

            host.MethodCallReplacementProvider = new SampleMethodReplacementProvider(interceptor);

            var targetMethod = modifiedTargetType.GetMethod("DoSomething");
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

        [Test]
        public void ShouldNotInterceptConstructorsWhenIntereptingAllMethodCalls()
        {
            var modifiedTargetType = GetModifiedTargetType((name, type) => type.InterceptAllMethodCalls());
            var instance = Activator.CreateInstance(modifiedTargetType);
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            AroundInvokeMethodCallRegistry.AddProvider(provider);
            var targetMethod = modifiedTargetType.GetMethod("DoSomething");
            targetMethod.Invoke(instance, null);

            Assert.IsTrue(aroundInvoke.BeforeInvokeWasCalled);
            Assert.IsTrue(aroundInvoke.AfterInvokeWasCalled);
        }
    }
}