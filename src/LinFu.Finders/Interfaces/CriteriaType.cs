using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Finders.Interfaces
{
    /// <summary>
    /// The enumeration that determines how a <see cref="ICriteria{T}"/> instance should
    /// be handled if the criteria test fails.
    /// </summary>
    public enum CriteriaType
    {
        /// <summary>
        /// A failure in a criteria test will result in a lower weighted
        /// score for a target item.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// A failure in a criteria test will be ignored, and hence,
        /// the criteria will be optional.
        /// </summary>
        Optional = 1,

        /// <summary>
        /// A failure in a criteria test will cause all previous and remaining
        /// tests against the criteria to fail.
        /// </summary>
        Critical = 2
    }
}
