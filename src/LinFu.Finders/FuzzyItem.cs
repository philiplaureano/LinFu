using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Finders.Interfaces;

namespace LinFu.Finders
{
    /// <summary>
    /// Represents the default implementation of a weighted item in
    /// a fuzzy list.
    /// </summary>
    /// <typeparam name="T">The item type to be tested.</typeparam>
    public class FuzzyItem<T> : IFuzzyItem<T>
    {
        private readonly T _item;
        private int _testCount;
        private int _matches;
        private bool _failed;

        /// <summary>
        /// Initializes the <see cref="FuzzyItem{T}"/> class with the given <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An instance of the <typeparamref name="T">item type</typeparamref> that will be tested.</param>
        public FuzzyItem(T item)
        {
            _item = item;
            _failed = false;
        }

        /// <summary>
        /// Reports the probability of a match
        /// based on the <see cref="ICriteria{T}"/>
        /// that has been tested so far. 
        /// 
        /// A value of 1.0 indicates a 100% match;
        /// A value of 0.0 equals a zero percent match.
        /// </summary>
        public double Confidence
        {
            get
            {
                if (_failed)
                    return -1;

                double result = 0;
                if (_testCount == 0)
                    return 0;

                result = ((double)_matches) / ((double)_testCount);

                return result;
            }
        }

        /// <summary>
        /// Gets the target item.
        /// </summary>
        public T Item
        {
            get { return _item; }
        }

        /// <summary>
        /// Tests if the current item matches the given
        /// <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="ICriteria{T}"/> that determines whether or not the <see cref="Item"/> meets a particular description.</param>
        public void Test(ICriteria<T> criteria)
        {
            // Determine the weight multiplier of this test
            var weight = criteria.Weight;

            // Ignore any further criteria tests
            // if this item fails
            if (_failed)
                return;

            var predicate = criteria.Predicate;
            if (predicate == null)
                return;

            var result = predicate(_item);

            // If the critical test fails, all matches will be reset
            // to zero and no further matches will be counted
            if (result == false && criteria.Type == CriteriaType.Critical)
            {
                _failed = true;
                return;
            }

            if (result)
                _matches += weight;

            // Don't count the result if the criteria
            // is optional
            if (result != true && criteria.Type == CriteriaType.Optional)
                return;

            _testCount += weight;
        }

        /// <summary>
        /// Resets the item back to its initial state.
        /// </summary>
        public void Reset()
        {
            _testCount = 0;
            _matches = 0;
            _failed = false;
        }
    }
}
