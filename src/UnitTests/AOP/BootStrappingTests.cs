using System.Collections.Generic;
using System.Linq;
using LinFu.AOP.Interfaces;
using NUnit.Framework;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class BootStrappingTests : BaseTestFixture
    {
        [Test]
        public void ShouldCallTypeThatImplementsBootstrapInterface()
        {
            BootStrapRegistry registry = BootStrapRegistry.Instance;
            IEnumerable<IBootStrappedComponent> bootStrappedComponents = registry.GetComponents();
            IBootStrappedComponent targetComponent = (from c in bootStrappedComponents
                                                      let type = c.GetType()
                                                      where type == typeof (SampleBootstrapComponent)
                                                      select c).First();

            var component = (SampleBootstrapComponent) targetComponent;
            Assert.IsTrue(component.Called);
        }
    }
}