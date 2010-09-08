using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Finders.Interfaces;

namespace LinFu.Finders
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ICriteria{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of item to test.</typeparam>
    public class Criteria<T> : ICriteria<T>
    {
        /// <summary>
        /// Gets or sets a value indicating the <see cref="CriteriaType"/>
        /// of the current <see cref="Criteria{T}"/>.
        /// </summary>
        public CriteriaType Type { get; set; }

        /// <summary>
        /// The condition that will determine whether or not
        /// the target item matches the criteria.
        /// </summary>
        public Func<T, bool> Predicate
        {
            get; set;
        }

        /// <summary>
        /// The weight of the given <see cref="Predicate"/>.
        /// </summary>
        public int Weight
        {
            get; set;
        }
    }
}
