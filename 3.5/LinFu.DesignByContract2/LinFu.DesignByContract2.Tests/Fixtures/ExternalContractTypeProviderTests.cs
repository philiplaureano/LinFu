using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Injectors;
using NMock2;
using NUnit.Framework;
using LinFu.DesignByContract2.Attributes;

namespace LinFu.DesignByContract2.Tests
{
    [TestFixture]
    public class ExternalContractTypeProviderTests : BaseFixture
    {
        [Test]
        public void ShouldQueryContractStorageForType()
        {
            Type targetType = typeof (object);
            IContractStorage storage = mock.NewMock<IContractStorage>();
            Expect.Once.On(storage).Method("HasContractFor").With(targetType).Will(Return.Value(false));
            ExternalContractTypeProvider provider = new ExternalContractTypeProvider();
            provider.ContractStorage = storage;

            IContractSource result = provider.ProvideContractForType(targetType);
            Assert.IsNotNull(result);
        }
    }
}
