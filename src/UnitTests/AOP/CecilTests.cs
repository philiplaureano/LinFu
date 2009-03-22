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
    }
}
