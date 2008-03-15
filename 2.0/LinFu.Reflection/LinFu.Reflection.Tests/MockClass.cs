using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Reflection.Tests
{
    public class MockClass : ITest 
    {
        private int _callCount;
        public int CallCount
        {
            get
            {
                return _callCount;
            }
        }
        public void Reset()
        {
            _callCount = 0;
        }
        #region ITest Members

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

        #endregion

        #region ITest Members


        public void TargetMethod<T>()
        {
            _callCount++;
        }

        #endregion
    }
}
