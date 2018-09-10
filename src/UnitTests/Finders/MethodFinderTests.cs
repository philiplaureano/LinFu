using System.Linq;
using System.Reflection;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using Xunit;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.Finders
{
    public class MethodFinderTests
    {
        [Fact]
        public void ShouldFindGenericMethod()
        {
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");

            var context = new MethodFinderContext(new[] {typeof(object)}, new object[0], typeof(void));
            var methods =
                typeof(SampleClassWithGenericMethod).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var finder = container.GetService<IMethodFinder<MethodInfo>>();
            var result = finder.GetBestMatch(methods, context);

            Assert.True(result.IsGenericMethod);
            Assert.True(result.GetGenericArguments().Count() == 1);
        }
    }
}