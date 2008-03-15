using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Tests
{
    public class TestImpl : ITest 
    {
        #region ITest Members
        
        public void DoSomething()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [TestPreconditionOne,
         TestPreconditionTwo]
        public void DoSomethingElse()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public object ReturnSomething()
        {
            throw new NotImplementedException();
        }
        public void DoSomethingWithParameterPreconditions(object arg1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
