using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Finders.Interfaces
{
    /// <summary>
    /// Represents a search item in a fuzzy search list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFuzzyItem<T>
    {
        /// <summary>
        /// Reports the probability of a match
        /// based on the <see cref="ICriteria{T}"/>
        /// that has been tested so far. 
        /// 
        /// A value of 1.0 indicates a 100% match;
        /// A value of 0.0 equals a zero percent match.
        /// </summary>
        double Confidence { get; }

        /// <summary>
        /// Gets the target item.
        /// </summary>
        T Item { get; }

        /// <summary>
        /// Tests if the current item matches the given
        /// <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="ICriteria{T}"/> that determines whether or not the <see cref="Item"/> meets a particular description.</param>
        void Test(ICriteria<T> criteria);

        /// <summary>
        /// Resets the item back to its initial state.
        /// </summary>
        void Reset();
    }
}
