using System;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class NewOperatorInterceptionTests : BaseTestFixture
    {
        private class OtherSampleService : ISampleService
        {
            #region ISampleService Members

            public void DoSomething()
            {
            }

            #endregion
        }

        [Test]
        public void ShouldInterceptObjectInstantiation()
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly("SampleLibrary.dll");

            ModuleDefinition module = assembly.MainModule;
            string typeName = "SampleClassWithNewInstanceCall";
            TypeDefinition targetType = (from TypeDefinition t in module.Types
                                         where t.Name == typeName
                                         select t).First();

            targetType.InterceptNewInstances(declaringType => declaringType.Name == "SampleServiceImplementation");

            Assembly modifiedAssembly = assembly.ToAssembly();

            Type modifiedTargetType = modifiedAssembly.GetTypes().Where(t => t.Name == typeName).First();
            object instance = Activator.CreateInstance(modifiedTargetType);
            Assert.IsNotNull(instance);

            // The activator will return a new OtherSampleService() instance instead
            // of a SampleServiceImplementation instance if the interception works
            var activator = new SampleTypeActivator(context => new OtherSampleService());
            var host = (IActivatorHost) instance;
            host.Activator = activator;

            MethodInfo targetMethod = modifiedTargetType.GetMethod("DoSomething");
            object result = targetMethod.Invoke(instance, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is OtherSampleService);
        }
    }
}