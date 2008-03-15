using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Common
{
    public class FuzzyFinder<T>
    {
        private double _tolerance = .51;
        public double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; }
        }

        public T Find(Predicate<T> predicate, IEnumerable<T> items)
        {
            // Build the list of items to match
            List<FuzzyItem<T>> itemList = new FuzzyItemList<T>(items);

            TestItems(itemList, predicate);

            FuzzyItem<T> bestMatch = GetBestMatch(itemList);

            if (bestMatch == null)
                return default(T);

            return bestMatch.Item;
        }

        private static void TestItems(List<FuzzyItem<T>> itemList, Predicate<T> predicate)
        {
            // Search for a match
            foreach (Predicate<T> currentPredicate in predicate.GetInvocationList())
            {
                foreach (FuzzyItem<T> currentItem in itemList)
                {
                    currentItem.Test(currentPredicate);
                }
            }
        }

        private FuzzyItem<T> GetBestMatch(List<FuzzyItem<T>> itemList)
        {
            FuzzyItem<T> bestMatch = null;
            double bestScore = 0;
            foreach (FuzzyItem<T> item in itemList)
            {
                if (item.Confidence <= bestScore)
                    continue;

                // Items that don't meet the minimum
                // tolerance level will be ignored
                if (item.Confidence < Tolerance)
                    continue;

                bestMatch = item;
                bestScore = item.Confidence;
            }
            return bestMatch;
        }
    }
}
