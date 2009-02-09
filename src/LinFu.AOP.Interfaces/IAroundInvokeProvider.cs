using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that injects code around a method implementation.
    /// </summary>
    public interface IAroundInvokeProvider
    {
        /// <summary>
        /// Gets the <see cref="IAroundInvoke"/> instance that will be executed
        /// before and after the target method (specified in the <paramref name="context"/> parameter)
        /// is called.
        /// </summary>
        /// <param name="context">The <see cref="IInvocationInfo"/> that describes the context of the method call at the call site.</param>        /// <returns>An <see cref="IAroundInvoke"/> instance if the surrounding behavior can be found; otherwise, it should return <c>null</c>.</returns>
        IAroundInvoke GetSurroundingImplementation(IInvocationInfo context);
    }
}
