using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an adapter class that maps a functor to an <see cref="IFieldFilter"/> instance.
    /// </summary>
    public class FieldFilterAdapter : IFieldFilter
    {
        private Func<FieldReference, bool> _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldFilterAdapter"/> class.
        /// </summary>
        /// <param name="filter">The field filter.</param>
        public FieldFilterAdapter(Func<FieldReference, bool> filter)
        {
            _filter = filter;
        }

        public bool ShouldWeave(MethodReference hostMethod, FieldReference targetField)
        {
            return _filter(targetField);
        }
    }
}
