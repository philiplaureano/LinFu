using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Proxy;
using NUnit.Framework;
using SampleLibrary;

namespace LinFu.UnitTests.Proxy
{
    [TestFixture]
    public class InterfaceExtractorTests
    {
        [Test]
        public void InterfaceExtractorShouldReturnTheCorrectResults()
        {
            var baseType = typeof (SampleClass);
            var extractor = new InterfaceExtractor();
            var interfaces = new HashSet<Type>();

            extractor.GetInterfaces(baseType, interfaces);

            Assert.IsTrue(interfaces.Contains(typeof (ISampleService)));
            Assert.IsTrue(interfaces.Contains(typeof (ISampleGenericService<int>)));

            // The result list must only contain interface types
            var nonInterfaceTypes = from t in interfaces
                                    where !t.IsInterface
                                    select t;

            Assert.IsTrue(nonInterfaceTypes.Count() == 0);
        }
    }
}
