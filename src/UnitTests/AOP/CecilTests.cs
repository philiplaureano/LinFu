using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Interfaces;
using LinFu.IoC.Configuration;
using Mono.Cecil;
using Moq;
using NUnit.Framework;
using SampleStronglyNamedLibrary;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class CecilTests : BaseTestFixture
    {
        private IServiceContainer _container;
        public override void Init()
        {
            _container = new ServiceContainer();
            _container.LoadFromBaseDirectory("*.dll");
        }

        public override void Term()
        {
            _container = null;
        }

        [Test]
        public void TypeWeaverShouldBeCalled()
        {
            var mockWeaver = new Mock<ITypeWeaver>();
            mockWeaver.Expect(weaver => weaver.ShouldWeave(It.IsAny<TypeDefinition>())).Returns(true);
            mockWeaver.Expect(weaver => weaver.AddAdditionalMembers(It.IsAny<ModuleDefinition>()));
            mockWeaver.Expect(weaver => weaver.ImportReferences(It.IsAny<ModuleDefinition>()));
            mockWeaver.Expect(weaver => weaver.Weave(It.IsAny<TypeDefinition>()));

            var location = typeof (SampleHelloClass).Assembly.Location;
            var assembly = AssemblyFactory.GetAssembly(location);
            var module = assembly.MainModule;

            module.WeaveWith(mockWeaver.Object);
        }

        [Test]
        public void MethodWeaverShouldBeCalled()
        {
            var mockWeaver = new Mock<IMethodWeaver>();
            
            
            var location = typeof(SampleHelloClass).Assembly.Location;
            var assembly = AssemblyFactory.GetAssembly(location);
            var module = assembly.MainModule;

            // Apply the weaver to the sample type
            var sampleType = (from TypeDefinition type in module.Types
                             where type.Name == "SampleHelloClass"
                             select type).First();

            mockWeaver.Expect(weaver => weaver.ShouldWeave(It.IsAny<MethodDefinition>())).Returns(true);
            mockWeaver.Expect(weaver => weaver.AddAdditionalMembers(sampleType));
            mockWeaver.Expect(weaver => weaver.ImportReferences(module));
            mockWeaver.Expect(weaver => weaver.Weave(It.IsAny<MethodDefinition>()));

            sampleType.WeaveWith(mockWeaver.Object);
        }
    }
}
