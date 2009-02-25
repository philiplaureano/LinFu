using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that is invoked before a method call.
    /// </summary>
    public interface IBeforeInvoke
    {
        /// <summary>
        /// This method will be called just before the actual
        /// method call is executed.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <seealso cref="IInvocationInfo"/>
        void BeforeInvoke(IInvocationInfo info);
    }
}
