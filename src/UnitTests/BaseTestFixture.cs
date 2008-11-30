using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
