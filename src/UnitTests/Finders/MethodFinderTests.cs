using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using NUnit.Framework;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.Finders
{
    [TestFixture]
    public class MethodFinderTests
    {
        [Test]
        public void ShouldFindGenericMethod()
        {
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");

            var context = new MethodFinderContext(new Type[]{typeof(object)}, new object[0], typeof(void));
            var methods = typeof(SampleClassWithGenericMethod).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var finder = container.GetService<IMethodFinder<MethodInfo>>();
            var result = finder.GetBestMatch(methods, context);

            Assert.IsTrue(result.IsGenericMethod);
            Assert.IsTrue(result.GetGenericArguments().Count() == 1);
        }
    }
}
