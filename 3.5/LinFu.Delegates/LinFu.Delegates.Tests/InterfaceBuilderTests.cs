using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace LinFu.Delegates.Tests
{
    [TestFixture]
    public class InterfaceBuilderTests : BaseFixture
    {
        [Test]
        public void ShouldGenerateAnonymousInterface()
        {
            Type[] parameters = new Type[0];
            Type returnType = typeof (void);
            Type result = InterfaceBuilder.DefineInterfaceMethod(returnType, parameters);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsInterface);
        }
    }
}
