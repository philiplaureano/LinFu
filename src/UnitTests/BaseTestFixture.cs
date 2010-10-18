using NUnit.Framework;

namespace LinFu.UnitTests
{
    public abstract class BaseTestFixture
    {
        [SetUp]
        public virtual void Init()
        {
        }

        [TearDown]
        public virtual void Term()
        {
        }
    }
}