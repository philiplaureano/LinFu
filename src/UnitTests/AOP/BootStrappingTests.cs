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
            var registry = BootStrapRegistry.Instance;
            var bootStrappedComponents = registry.GetComponents();
            var targetComponent = (from c in bootStrappedComponents
                let type = c.GetType()
                where type == typeof(SampleBootstrapComponent)
                select c).First();

            var component = (SampleBootstrapComponent) targetComponent;
            Assert.IsTrue(component.Called);
        }
    }
}