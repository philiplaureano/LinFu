using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public class Counter<T>
    {
        private int _count;
        private object lockObject = new object();
        public void Increment()
        {
            lock (lockObject)
            {
                _count++;
            }
        }
        public void Decrement()
        {
            lock (lockObject)
            {
                _count--;
            }
        }
        public int GetCount()
        {
            return _count;
        }
    }
}
