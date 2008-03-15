using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Common
{
    public class FuzzyItemList<T> : List<FuzzyItem<T>>
    {
        public FuzzyItemList(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(new FuzzyItem<T>(item));
            }
        }
    }
}
