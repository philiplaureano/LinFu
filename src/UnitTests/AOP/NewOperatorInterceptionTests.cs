using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using NUnit.Framework;
using Moq;
using SampleLibrary;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class NewOperatorInterceptionTests : BaseTestFixture
    {
        private class OtherSampleService : ISampleService
        {
            public void DoSomething()
            {
            }
        }

        [Test]
        public void ShouldInterceptObjectInstantiation()
        {
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");

            var module = assembly.MainModule;
            var typeName = "SampleClassWithNewInstanceCall";
            var targetType = (from TypeDefinition t in module.Types
                              where t.Name == typeName
                              select t).First();

            targetType.InterceptNewInstances(declaringType => declaringType.Name == "SampleServiceImplementation");

            var modifiedAssembly = assembly.ToAssembly();

            var modifiedTargetType = modifiedAssembly.GetTypes().Where(t => t.Name == typeName).First();
            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.IsNotNull(instance);

            // The activator will return a new OtherSampleService() instance instead
            // of a SampleServiceImplementation instance if the interception works
            var activator = new SampleTypeActivator(context => new OtherSampleService());
            var host = (IActivatorHost)instance;
            host.Activator = activator;

            var targetMethod = modifiedTargetType.GetMethod("DoSomething");
            var result = targetMethod.Invoke(instance, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is OtherSampleService);
        }
    }
}
