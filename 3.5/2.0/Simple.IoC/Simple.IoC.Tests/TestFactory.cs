using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Tests
{
    [Factory(typeof(ITestObject))]
    public class TestFactory : IFactory<ITestObject>
    {
        #region IFactory<ITestObject> Members

        public ITestObject CreateInstance(IContainer container)
        {
            return null;
        }

        #endregion
    }
}
