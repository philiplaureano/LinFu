using System;
using System.Linq;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Reflection;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Xunit;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    public class MethodBodyInterceptionTests
    {
        private void Test(Action<object> testInstance)
        {
            var libraryFileName = "SampleLibrary.dll";
            var typeName = "SampleClassWithNonVirtualMethod";
            Func<MethodReference, bool> methodFilter = m => m.Name == "DoSomething";

            Test(libraryFileName, typeName, methodFilter, type => Test(type, testInstance));
        }

        private void Test(string libraryFileName, string typeName, Func<MethodReference, bool> methodFilter,
            Action<Type> testTargetType)
        {
            var assembly = AssemblyDefinition.ReadAssembly(libraryFileName);
            var module = assembly.MainModule;

            var targetType = (from TypeDefinition t in module.Types
                where t.Name == typeName
                select t).First();

            Assert.NotNull(targetType);

            ModifyType(targetType, methodFilter);

            var modifiedTargetType = CreateModifiedType(assembly, typeName);

            testTargetType(modifiedTargetType);
        }

        private void Test(Type modifiedTargetType, Action<object> testInstance)
        {
            var instance = Activator.CreateInstance(modifiedTargetType);
            testInstance(instance);
        }

        private Type CreateModifiedType(AssemblyDefinition assembly, string typeName)
        {
            var modifiedAssembly = assembly.ToAssembly();

            return (from t in modifiedAssembly.GetTypes()
                where t.Name == typeName
                select t).First();
        }

        private void ModifyType(TypeDefinition targetType, Func<MethodReference, bool> methodFilter)
        {
            targetType.InterceptMethodBody(methodFilter);
        }

        [Fact]
        public void ShouldImplementIModifiableTypeOnModifiedSampleClass()
        {
            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);
                Assert.True(instance is IModifiableType);
            };

            Test(condition);
        }

        [Fact]
        public void ShouldInterceptStaticMethodWithAroundInvokeProvider()
        {
            Func<MethodReference, bool> methodFilter = m => m.Name == "DoSomething";

            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            AroundMethodBodyRegistry.AddProvider(provider);
            Action<Type> doTest = type =>
            {
                var doSomethingMethod = type.GetMethod("DoSomething");
                Assert.NotNull(doSomethingMethod);

                doSomethingMethod.Invoke(null, new object[0]);
                Assert.True(aroundInvoke.BeforeInvokeWasCalled);
                Assert.True(aroundInvoke.AfterInvokeWasCalled);
            };

            Test("SampleLibrary.dll", "SampleStaticClassWithStaticMethod", methodFilter, doTest);
        }

        [Fact]
        public void ShouldInvokeAroundInvokeProviderIfInterceptionIsEnabled()
        {
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);
            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);

                var modifiedInstance = (IModifiableType) instance;
                modifiedInstance.AroundMethodBodyProvider = provider;

                instance.Invoke("DoSomething");

                Assert.True(aroundInvoke.BeforeInvokeWasCalled);
                Assert.True(aroundInvoke.AfterInvokeWasCalled);
            };

            Test(condition);
        }

        [Fact]
        public void ShouldInvokeClassAroundInvokeProviderIfInterceptionIsEnabled()
        {
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);

                AroundMethodBodyRegistry.AddProvider(provider);
                instance.Invoke("DoSomething");

                Assert.True(aroundInvoke.BeforeInvokeWasCalled);
                Assert.True(aroundInvoke.AfterInvokeWasCalled);
            };

            Test(condition);
        }

        [Fact]
        public void ShouldInvokeClassMethodReplacementProviderIfInterceptionIsEnabled()
        {
            Func<MethodReference, bool> methodFilter = m => m.Name == "DoSomething";
            var replacement = new SampleMethodReplacement();
            var provider = new SampleMethodReplacementProvider(replacement);

            MethodBodyReplacementProviderRegistry.SetProvider(provider);
            Action<Type> doTest = type =>
            {
                var doSomethingMethod = type.GetMethod("DoSomething");
                Assert.NotNull(doSomethingMethod);

                doSomethingMethod.Invoke(null, new object[0]);
            };

            Test("SampleLibrary.dll", "SampleStaticClassWithStaticMethod", methodFilter, doTest);
            Assert.True(replacement.HasBeenCalled);
        }

        [Fact]
        public void ShouldInvokeMethodBodyReplacementIfInterceptionIsEnabled()
        {
            var sampleInterceptor = new SampleInterceptor();
            var sampleProvider = new SampleMethodReplacementProvider(sampleInterceptor);

            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);
                Assert.True(instance is IModifiableType);

                var modifiableType = (IModifiableType) instance;
                modifiableType.MethodBodyReplacementProvider = sampleProvider;
                modifiableType.IsInterceptionDisabled = false;

                instance.Invoke("DoSomething");
            };

            Test(condition);
            Assert.True(sampleInterceptor.HasBeenInvoked);
        }

        [Fact]
        public void ShouldNotImplementIModifiableTypeOnStaticClasses()
        {
            Func<MethodReference, bool> methodFilter = m => m.Name == "DoSomething";

            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            AroundMethodBodyRegistry.AddProvider(provider);
            Action<Type> doTest = type =>
            {
                var doSomethingMethod = type.GetMethod("DoSomething");
                Assert.NotNull(doSomethingMethod);
                Assert.DoesNotContain(typeof(IModifiableType), type.GetInterfaces());
            };

            Test("SampleLibrary.dll", "SampleStaticClassWithStaticMethod", methodFilter, doTest);
        }

        [Fact]
        public void ShouldNotInvokeAroundInvokeProviderIfInterceptionIsDisabled()
        {
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);
            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);

                var modifiedInstance = (IModifiableType) instance;
                modifiedInstance.AroundMethodBodyProvider = provider;
                modifiedInstance.IsInterceptionDisabled = true;

                instance.Invoke("DoSomething");

                Assert.False(aroundInvoke.BeforeInvokeWasCalled);
                Assert.False(aroundInvoke.AfterInvokeWasCalled);
            };

            Test(condition);
        }

        [Fact]
        public void ShouldNotInvokeClassAroundInvokeProviderIfInterceptionIsDisabled()
        {
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);

            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);

                var modified = (IModifiableType) instance;
                modified.IsInterceptionDisabled = true;

                AroundMethodBodyRegistry.AddProvider(provider);
                instance.Invoke("DoSomething");

                Assert.False(aroundInvoke.BeforeInvokeWasCalled);
                Assert.False(aroundInvoke.AfterInvokeWasCalled);
            };

            Test(condition);
        }

        [Fact]
        public void ShouldNotInvokeClassMethodReplacementProviderIfInterceptionIsDisabled()
        {
            var sampleInterceptor = new SampleInterceptor();
            var sampleProvider = new SampleMethodReplacementProvider(sampleInterceptor);
            MethodBodyReplacementProviderRegistry.SetProvider(sampleProvider);

            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);

                var modified = (IModifiableType) instance;
                modified.IsInterceptionDisabled = true;


                instance.Invoke("DoSomething");
                Assert.False(sampleInterceptor.HasBeenInvoked);
            };

            Test(condition);
        }

        [Fact]
        public void ShouldNotInvokeMethodBodyReplacementIfInterceptionIsDisabled()
        {
            var sampleInterceptor = new SampleInterceptor();
            var sampleProvider = new SampleMethodReplacementProvider(sampleInterceptor);

            Action<object> condition = instance =>
            {
                Assert.NotNull(instance);
                Assert.True(instance is IModifiableType);

                var modifiableType = (IModifiableType) instance;
                modifiableType.MethodBodyReplacementProvider = sampleProvider;
                modifiableType.IsInterceptionDisabled = true;

                instance.Invoke("DoSomething");
            };

            Test(condition);
            Assert.False(sampleInterceptor.HasBeenInvoked);
        }
    }
}