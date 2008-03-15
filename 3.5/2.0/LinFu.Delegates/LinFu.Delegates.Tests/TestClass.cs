using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Delegates.Tests
{
    internal class TestClass
    {
        private static int _staticCallCount;
        private int _callCount;
        public int CallCount
        {
            get
            {
                return _callCount;
            }
        }
        public static int StaticCallCount
        {
            get { return _staticCallCount;  }   
        }
        public void Reset()
        {
            _callCount = 0;
            _staticCallCount = 0;
        }
        public static void StaticTargetMethod()
        {
            _staticCallCount++;
        }
        public void TargetMethod()
        {
            _callCount++;
        }

        public object TargetProperty
        {
            get
            {
                _callCount++;
                return null;
            }
            set
            {
                _callCount++;
            }
        }


        
    }
}
