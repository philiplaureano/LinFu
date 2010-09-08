using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Finders.Interfaces;

namespace LinFu.Finders
{
    /// <summary>
    /// A class that adds fuzzy search support to <see cref="IList{T}"/> instances.
    /// </summary>
    public static class FinderExtensions
    {
        /// <summary>
        /// Applies a criteria to the <paramref name="list"/> of 
        /// fuzzy items.
        /// </summary>
        /// <typeparam name="TItem">The type of item to test.</typeparam>
        /// <param name="list">The list of <see cref="IFuzzyItem{T}"/> instances that represent a single test case in a fuzzy search.</param>
        /// <param name="criteria">The criteria to test against each item in the list.</param>
        public static void AddCriteria<TItem>(this IList<IFuzzyItem<TItem>> list, ICriteria<TItem> criteria)
        {
            foreach (var item in list)
            {
                if (item == null)
                    continue;

                item.Test(criteria);
            }
        }
        /// <summary>
        /// Applies a criteria to the <paramref name="list"/> of 
        /// fuzzy items using the given <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of item to test.</typeparam>
        /// <param name="list">The list of <see cref="IFuzzyItem{T}"/> instances that represent a single test case in a fuzzy search.</param>
        /// <param name="predicate">The condition that will be used to test the target item.</param>        
        public static void AddCriteria<TItem>(this IList<IFuzzyItem<TItem>> list, Func<TItem, bool> predicate)
        {
            list.AddCriteria(predicate, CriteriaType.Standard);
        }

        /// <summary>
        /// Applies a criteria to the <paramref name="list"/> of 
        /// fuzzy items using the given <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of item to test.</typeparam>
        /// <param name="list">The list of <see cref="IFuzzyItem{T}"/> instances that represent a single test case in a fuzzy search.</param>
        /// <param name="predicate">The condition that will be used to test the target item.</param>        
        /// <param name="criteriaType">The <see cref="CriteriaType"/> to associate with the predicate.</param>        
        public static void AddCriteria<TItem>(this IList<IFuzzyItem<TItem>> list, Func<TItem, bool> predicate,
            CriteriaType criteriaType)
        {
            const int defaultWeight = 1;
            list.AddCriteria(predicate, criteriaType, defaultWeight);
        }

        /// <summary>
        /// Applies a criteria to the <paramref name="list"/> of 
        /// fuzzy items using the given <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of item to test.</typeparam>
        /// <param name="list">The list of <see cref="IFuzzyItem{T}"/> instances that represent a single test case in a fuzzy search.</param>
        /// <param name="predicate">The condition that will be used to test the target item.</param>        
        /// <param name="criteriaType">The <see cref="CriteriaType"/> to associate with the predicate.</param>        
        /// <param name="weight">The weight of the predicate value expressed in the number of tests that will be counted for/against the target item as a result of the predicate.</param>
        public static void AddCriteria<TItem>(this IList<IFuzzyItem<TItem>> list, Func<TItem, bool> predicate, CriteriaType criteriaType, int weight)
        {
            var criteria = new Criteria<TItem>()
                               {
                                   Predicate = predicate,
                                   Weight = weight,
                                   Type = criteriaType
                               };

            list.AddCriteria(criteria);
        }

        /// <summary>
        /// Adds an item to a fuzzy list.
        /// </summary>
        /// <typeparam name="T">The type of the item being added.</typeparam>
        /// <param name="list">The fuzzy list that will contain the new item.</param>
        /// <param name="item">The item being added.</param>
        public static void Add<T>(this IList<IFuzzyItem<T>> list, T item)
        {
            list.Add(new FuzzyItem<T>(item));
        }

        /// <summary>
        /// Returns the FuzzyItem with the highest confidence score in a given
        /// <see cref="IFuzzyItem{T}"/> list.
        /// </summary>
        /// <typeparam name="TItem">The type of item being compared.</typeparam>
        /// <param name="list">The fuzzy list that contains the list of possible matches.</param>
        /// <returns>The item with the highest match.</returns>
        public static IFuzzyItem<TItem> BestMatch<TItem>(this IList<IFuzzyItem<TItem>> list)
        {
            double bestScore = 0;
            IFuzzyItem<TItem> bestMatch = null;
            foreach (var item in list)
            {
                if (item.Confidence <= bestScore)
                    continue;

                bestMatch = item;
                bestScore = item.Confidence;
            }

            return bestMatch;
        }
        
        /// <summary>
        /// Resets the scores of all fuzzy items in the current list.
        /// </summary>
        /// <typeparam name="TItem">The target item type.</typeparam>
        /// <param name="list">The fuzzy list itself.</param>
        public static void Reset<TItem>(this IList<IFuzzyItem<TItem>> list)
        {
            foreach(var item in list)
            {
                item.Reset();
            }
        }
        /// <summary>
        /// Converts a list into a list of <see cref="IFuzzyItem{T}"/> objects.
        /// </summary>
        /// <typeparam name="TItem">The item type will be used in the fuzzy search.</typeparam>
        /// <param name="items">The target list to be converted.</param>
        /// <returns>A fuzzy list containing the elements from the given list.</returns>
        public static IList<IFuzzyItem<TItem>> AsFuzzyList<TItem>(this IEnumerable<TItem> items)
        {
            var result = new List<IFuzzyItem<TItem>>();
            foreach(var item in items)
            {
                result.Add(item);
            }

            return result;
        }
    }
}
