using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Tests
{
    public class TestChild2 : TestBase
    {
        [TestPreconditionTwo]
        public override void DoSomething()
        {
        }
    }
}
