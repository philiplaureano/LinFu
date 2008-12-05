using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using Moq;
using NUnit.Framework;
using LinFu.IoC.Interfaces;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class FactoryStorageTests : BaseTestFixture
    {
        private IFactoryStorage _storage;

        public override void Init()
        {
            _storage = new FactoryStorage();
        }

        public override void Term()
        {
            _storage = null;
        }

        [Test]
        public void ShouldDistinguishBetweenTwoServicesOfTheSameTypeButDifferentParameters()
        {
            var firstFactory = new Mock<IFactory>();
            var secondFactory = new Mock<IFactory>();

            var serviceType = typeof (int);

            IEnumerable<Type> firstParameters = new Type[] { typeof(int), typeof(int) };
            IEnumerable<Type> secondParameters = new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) };

            _storage.AddFactory("", serviceType, firstParameters, firstFactory.Object);
            _storage.AddFactory("", serviceType, secondParameters, secondFactory.Object);

            Assert.IsTrue(_storage.ContainsFactory("", serviceType, firstParameters));
            Assert.IsTrue(_storage.ContainsFactory("", serviceType, secondParameters));

            // Make sure that the factory returns the correct container
            var firstResult = _storage.GetFactory("", serviceType, firstParameters);
            Assert.AreSame(firstFactory.Object, firstResult);

            var secondResult = _storage.GetFactory("", serviceType, secondParameters);
            Assert.AreSame(secondFactory.Object, secondResult);
        }
    }
}
