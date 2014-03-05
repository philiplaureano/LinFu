using System;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an adapter class that maps a functor to an <see cref="IFieldFilter"/> instance.
    /// </summary>
    public class FieldFilterAdapter : IFieldFilter
    {
        private readonly Func<FieldReference, bool> _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldFilterAdapter"/> class.
        /// </summary>
        /// <param name="filter">The field filter.</param>
        public FieldFilterAdapter(Func<FieldReference, bool> filter)
        {
            _filter = filter;
        }


        /// <summary>
        /// Determines whether or not a particular field get or set should be intercepted.
        /// </summary>
        /// <param name="hostMethod">The host method.</param>
        /// <param name="targetField">The target field.</param>
        /// <returns>Returns <c>true</c> if the field should be intercepted; otherwise, it will return <c>false</c>.</returns>
        public bool ShouldWeave(MethodReference hostMethod, FieldReference targetField)
        {
            return _filter(targetField);
        }
    }
}