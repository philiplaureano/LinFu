using LinFu.Reflection.Emit;
using Mono.Cecil;
using Xunit;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    public class ParameterDefinitionExtensionTests : BaseTestFixture
    {
        [Fact]
        public void ShouldBeAbleToDetermineIfMethodIsByRef()
        {
            var location = typeof(SampleClassWithByRefMethod).Assembly.Location;
            var assembly = AssemblyDefinition.ReadAssembly(location);
            var module = assembly.MainModule;

            var targetType = module.GetType("SampleClassWithByRefMethod");
            var byRefMethod = targetType.GetMethod("ByRefMethod");
            var regularMethod = targetType.GetMethod("NonByRefMethod");

            Assert.NotNull(assembly);
            Assert.NotNull(targetType);
            Assert.NotNull(byRefMethod);
            Assert.NotNull(regularMethod);

            // Test the byref parameter
            var parameter = byRefMethod.Parameters[0];
            Assert.True(parameter.IsByRef());

            // Test the non-byref parameter
            parameter = regularMethod.Parameters[0];
            Assert.False(parameter.IsByRef());
        }
    }
}