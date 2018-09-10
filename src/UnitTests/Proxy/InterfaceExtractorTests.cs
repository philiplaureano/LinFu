using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.Proxy;
using Xunit;
using SampleLibrary;

namespace LinFu.UnitTests.Proxy
{
    public class InterfaceExtractorTests
    {
        [Fact]
        public void InterfaceExtractorShouldReturnTheCorrectResults()
        {
            var baseType = typeof(SampleClass);
            var extractor = new InterfaceExtractor();
            var interfaces = new HashSet<Type>();

            extractor.GetInterfaces(baseType, interfaces);

            Assert.Contains(typeof(ISampleService), interfaces);
            Assert.Contains(typeof(ISampleGenericService<int>), interfaces);

            // The result list must only contain interface types
            var nonInterfaceTypes = from t in interfaces
                where !t.IsInterface
                select t;

            Assert.True(nonInterfaceTypes.Count() == 0);
        }
    }
}