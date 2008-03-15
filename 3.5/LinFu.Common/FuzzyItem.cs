using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Common
{
    public class FuzzyItem<T>
    {
        private int _testCount;
        private int _matches;
        private T _item;
        public FuzzyItem(T item)
        {
            _item = item;
        }
        public void Test(Predicate<T> predicate)
        {
            if (predicate == null)
                return;

            if (predicate(_item))
                _matches++;

            _testCount++;
        }

        public void Reset()
        {
            _testCount = 0;
            _matches = 0;
        }
        public double Confidence
        {
            get
            {
                double result = 0;
                if (_testCount == 0)
                    return 0;

                result = ((double)_matches) / ((double)_testCount);

                return result;
            }
        }
        public T Item
        {
            get { return _item; }
        }
    }
}
