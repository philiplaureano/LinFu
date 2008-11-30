using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var location = typeof (SampleClassWithByRefMethod).Assembly.Location;
            var assembly = AssemblyFactory.GetAssembly(location);
            var module = assembly.MainModule;

            var targetType = module.GetType("SampleClassWithByRefMethod");
            var byRefMethod = targetType.GetMethod("ByRefMethod");
            var regularMethod = targetType.GetMethod("NonByRefMethod");

            Assert.IsNotNull(assembly);
            Assert.IsNotNull(targetType);
            Assert.IsNotNull(byRefMethod);
            Assert.IsNotNull(regularMethod);

            // Test the byref parameter
            var parameter = byRefMethod.Parameters[0];
            Assert.IsTrue(parameter.IsByRef());

            // Test the non-byref parameter
            parameter = regularMethod.Parameters[0];
            Assert.IsFalse(parameter.IsByRef());
        }
    }
}
