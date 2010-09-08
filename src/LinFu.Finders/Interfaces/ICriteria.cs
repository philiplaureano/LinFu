using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Finders.Interfaces
{
    /// <summary>
    /// Represents a class that describes the search criteria
    /// for a given item <typeparamref name="T">type</typeparamref>.
    /// </summary>
    /// <typeparam name="T">The target item type.</typeparam>
    public interface ICriteria<T>
    {
        /// <summary>
        /// Gets or sets a value indicating the <see cref="CriteriaType"/>
        /// of the current <see cref="ICriteria{T}"/>.
        /// </summary>
        CriteriaType Type { get; set; }

        /// <summary>
        /// The condition that will determine whether or not
        /// the target item matches the criteria.
        /// </summary>
        Func<T, bool> Predicate { get; }

        /// <summary>
        /// Gets or sets a value indicating the weight of the given <see cref="Predicate"/>.
        /// </summary>
        int Weight { get; set; }
    }
}
