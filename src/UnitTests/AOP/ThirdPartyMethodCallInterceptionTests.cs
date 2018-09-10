using System;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Xunit;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    public class ThirdPartyMethodCallInterceptionTests : BaseTestFixture
    {
        protected override void Init()
        {
        }

        protected override void Term()
        {
            AroundInvokeMethodCallRegistry.Clear();
        }

        private Type GetModifiedTargetType()
        {
            return GetModifiedTargetType(Modify);
        }

        private Type GetModifiedTargetType(Action<string, TypeDefinition> modify)
        {
            var assembly = AssemblyDefinition.ReadAssembly("SampleLibrary.dll");
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

        [Fact]
        public void ShouldCallInstanceAroundInvokeProvider()
        {
            var modifiedTargetType = GetModifiedTargetType();
            var instance = Activator.CreateInstance(modifiedTargetType);
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            var modified = (IModifiableType) instance;
            modified.AroundMethodCallProvider = provider;
            var targetMethod = modifiedTargetType.GetMethod("DoSomething");

            Assert.NotNull(targetMethod);
            targetMethod.Invoke(instance, null);

            Assert.True(aroundInvoke.BeforeInvokeWasCalled);
            Assert.True(aroundInvoke.AfterInvokeWasCalled);
        }

        [Fact]
        public void ShouldCallStaticAroundInvokeProvider()
        {
            var modifiedTargetType = GetModifiedTargetType();
            var instance = Activator.CreateInstance(modifiedTargetType);
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            AroundInvokeMethodCallRegistry.AddProvider(provider);

            var targetMethod = modifiedTargetType.GetMethod("DoSomething");
            Assert.NotNull(targetMethod);
            
            targetMethod.Invoke(instance, null);

            Assert.True(aroundInvoke.BeforeInvokeWasCalled);
            Assert.True(aroundInvoke.AfterInvokeWasCalled);
        }

        [Fact]
        public void ShouldImplementIMethodReplacementHostOnTargetType()
        {
            var modifiedTargetType = GetModifiedTargetType();

            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.NotNull(instance);
            Assert.True(instance is IMethodReplacementHost);

            var host = (IMethodReplacementHost) instance;

            var interceptor = new SampleMethodReplacement();

            host.MethodCallReplacementProvider = new SampleMethodReplacementProvider(interceptor);

            var targetMethod = modifiedTargetType.GetMethod("DoSomething");
            Assert.NotNull(targetMethod);
            try
            {
                targetMethod.Invoke(instance, null);
            }
            catch (TargetInvocationException ex)
            {
                var innerException = ex.InnerException;
                Console.WriteLine(ex.ToString());

                if (innerException != null)
                    throw innerException;

                throw;
            }

            Assert.True(interceptor.HasBeenCalled);
        }

        [Fact]
        public void ShouldNotInterceptConstructorsWhenIntereptingAllMethodCalls()
        {
            var modifiedTargetType = GetModifiedTargetType((name, type) => type.InterceptAllMethodCalls());
            var instance = Activator.CreateInstance(modifiedTargetType);
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            AroundInvokeMethodCallRegistry.AddProvider(provider);
            var targetMethod = modifiedTargetType.GetMethod("DoSomething");
            Assert.NotNull(targetMethod);

            targetMethod.Invoke(instance, null);

            Assert.True(aroundInvoke.BeforeInvokeWasCalled);
            Assert.True(aroundInvoke.AfterInvokeWasCalled);
        }
    }
}