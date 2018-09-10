using System;
using System.Collections.Generic;
using LinFu.IoC;
using LinFu.IoC.Interfaces;
using Moq;
using Xunit;

namespace LinFu.UnitTests.IOC
{
    public class FactoryStorageTests : BaseTestFixture
    {
        protected override void Init()
        {
            _storage = new FactoryStorage();
        }

        protected override void Term()
        {
            _storage = null;
        }


        private IFactoryStorage _storage;

        [Fact]
        public void ShouldDistinguishBetweenTwoServicesOfTheSameTypeButDifferentParameters()
        {
            var firstFactory = new Mock<IFactory>();
            var secondFactory = new Mock<IFactory>();

            var serviceType = typeof(int);

            IEnumerable<Type> firstParameters = new[] {typeof(int), typeof(int)};
            IEnumerable<Type> secondParameters = new[] {typeof(int), typeof(int), typeof(int), typeof(int)};

            _storage.AddFactory("", serviceType, firstParameters, firstFactory.Object);
            _storage.AddFactory("", serviceType, secondParameters, secondFactory.Object);

            Assert.True(_storage.ContainsFactory("", serviceType, firstParameters));
            Assert.True(_storage.ContainsFactory("", serviceType, secondParameters));

            // Make sure that the factory returns the correct container
            var firstResult = _storage.GetFactory("", serviceType, firstParameters);
            Assert.Same(firstFactory.Object, firstResult);

            var secondResult = _storage.GetFactory("", serviceType, secondParameters);
            Assert.Same(secondFactory.Object, secondResult);
        }
    }
}