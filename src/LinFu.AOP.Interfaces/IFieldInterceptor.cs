using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that can intercept field getter and setter calls.
    /// </summary>
    public interface IFieldInterceptor
    {
        /// <summary>
        /// Determines whether or not a field can be intercepted.
        /// </summary>
        /// <param name="context">The context that describes the field to be intercepted.</param>
        /// <returns><c>true</c> if it can be intercepted; otherwise, it will return <c>false</c>.</returns>
        bool CanIntercept(IFieldInterceptionContext context);

        /// <summary>
        /// Gets the value of a field.
        /// </summary>
        /// <param name="context">The context that describes the field to be intercepted.</param>
        /// <returns>The value of the target field.</returns>
        object GetValue(IFieldInterceptionContext context);

        /// <summary>
        /// Sets the value of a field.
        /// </summary>
        /// <param name="context">The context that describes the field to be intercepted.</param>
        /// <param name="value">The original value that will be assigned to the target field.</param>
        /// <returns>The value that will be assigned to the target field.</returns>
        object SetValue(IFieldInterceptionContext context, object value);
    }
}
