using System.Linq;
using LinFu.AOP.Interfaces;
using Xunit;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    public class BootStrappingTests : BaseTestFixture
    {
        [Fact]
        public void ShouldCallTypeThatImplementsBootstrapInterface()
        {
            var registry = BootStrapRegistry.Instance;
            var bootStrappedComponents = registry.GetComponents();
            var targetComponent = (from c in bootStrappedComponents
                let type = c.GetType()
                where type == typeof(SampleBootstrapComponent)
                select c).First();

            var component = (SampleBootstrapComponent) targetComponent;
            Assert.True(component.Called);
        }
    }
}