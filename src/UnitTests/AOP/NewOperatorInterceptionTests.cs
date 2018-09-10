using System;
using System.Linq;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Xunit;
using SampleLibrary;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    public class NewOperatorInterceptionTests : BaseTestFixture
    {
        private class OtherSampleService : ISampleService
        {
            public void DoSomething()
            {
            }
        }

        [Fact]
        public void ShouldInterceptObjectInstantiation()
        {
            var assembly = AssemblyDefinition.ReadAssembly("SampleLibrary.dll");

            var module = assembly.MainModule;
            var typeName = "SampleClassWithNewInstanceCall";
            var targetType = (from TypeDefinition t in module.Types
                where t.Name == typeName
                select t).First();

            targetType.InterceptNewInstances(declaringType => declaringType.Name == "SampleServiceImplementation");

            var modifiedAssembly = assembly.ToAssembly();

            var modifiedTargetType = modifiedAssembly.GetTypes().First(t => t.Name == typeName);
            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.NotNull(instance);

            // The activator will return a new OtherSampleService() instance instead
            // of a SampleServiceImplementation instance if the interception works
            var activator = new SampleTypeActivator(context => new OtherSampleService());
            var host = (IActivatorHost) instance;
            host.Activator = activator;

            var targetMethod = modifiedTargetType.GetMethod("DoSomething");
            
            Assert.NotNull(targetMethod);
            
            var result = targetMethod.Invoke(instance, null);

            Assert.NotNull(result);
            Assert.True(result is OtherSampleService);
        }
    }
}