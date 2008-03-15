using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Tests
{
    public abstract class TestBase
    {
        [TestPreconditionOne]
        public virtual void DoSomething()
        {
            
        }
    }
}
