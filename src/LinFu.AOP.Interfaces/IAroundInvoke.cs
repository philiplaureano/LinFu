using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that can wrap itself around any given method call.
    /// </summary>
    public interface IAroundInvoke
    {
        /// <summary>
        /// This method will be called just before the actual
        /// method call is executed.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <seealso cref="IInvocationInfo"/>
        void BeforeInvoke(IInvocationInfo info);

        /// <summary>
        /// This method will be called immediately after the actual
        /// method call is executed.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <param name="returnValue">The value returned from the actual method call.</param>
        void AfterInvoke(IInvocationInfo info, object returnValue);
    }
}
