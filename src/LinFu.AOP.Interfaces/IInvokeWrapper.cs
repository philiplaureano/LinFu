using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a special type of interceptor that can
    /// wrap itself around a method call.
    /// </summary>
    public interface IInvokeWrapper : IAroundInvoke
    {        
        /// <summary>
        /// This method will provide the actual implementation
        /// for the <see cref="IInvocationInfo.TargetMethod">target method</see>
        /// instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <returns>The actual return value from the <see cref="IInvocationInfo.TargetMethod"/>.</returns>
        object DoInvoke(IInvocationInfo info);        
    }
}