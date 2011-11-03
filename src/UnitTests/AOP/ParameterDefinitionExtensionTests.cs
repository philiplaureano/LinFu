using LinFu.Reflection.Emit;
using Mono.Cecil;
using NUnit.Framework;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class ParameterDefinitionExtensionTests : BaseTestFixture
    {
        [Test]
        public void ShouldBeAbleToDetermineIfMethodIsByRef()
        {
            string location = typeof (SampleClassWithByRefMethod).Assembly.Location;
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(location);
            ModuleDefinition module = assembly.MainModule;

            TypeDefinition targetType = module.GetType("SampleLibrary.AOP.SampleClassWithByRefMethod");
            MethodDefinition byRefMethod = targetType.GetMethod("ByRefMethod");
            MethodDefinition regularMethod = targetType.GetMethod("NonByRefMethod");

            Assert.IsNotNull(assembly);
            Assert.IsNotNull(targetType);
            Assert.IsNotNull(byRefMethod);
            Assert.IsNotNull(regularMethod);

            // Test the byref parameter
            ParameterDefinition parameter = byRefMethod.Parameters[0];
            Assert.IsTrue(parameter.IsByRef());

            // Test the non-byref parameter
            parameter = regularMethod.Parameters[0];
            Assert.IsFalse(parameter.IsByRef());
        }
    }
}