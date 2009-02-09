using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that has been modified to support
    /// pervasive method interception.
    /// </summary>
    public interface IModifiableType
    {
        /// <summary>
        /// Gets or sets the value indicating whether or not 
        /// method interception should be disabled.
        /// </summary>
        bool IsInterceptionDisabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IAroundInvokeProvider"/>
        /// that will be used to inject code "around" a particular method body
        /// implementation.
        /// </summary>
        IAroundInvokeProvider AroundInvokeProvider { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IInterceptorProvider"/>
        /// that will be used to replace an existing method body implementation.
        /// </summary>
        IInterceptorProvider MethodReplacementProvider { get; set; }
    }
}
