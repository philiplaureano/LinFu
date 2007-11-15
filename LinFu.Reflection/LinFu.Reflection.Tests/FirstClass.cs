using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Reflection.Tests
{
    public class FirstClass
    {
        private int _callCount;
        public void TestMethod1()
        {
            _callCount++;
        }
        public int CallCount
        {
            get { return _callCount;  }
        }
        public void Reset()
        {
            _callCount = 0;
        }
    }
}
