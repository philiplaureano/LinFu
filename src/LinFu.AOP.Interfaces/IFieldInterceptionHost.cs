using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that has been modified to intercept field getters and setters.
    /// </summary>
    public interface IFieldInterceptionHost
    {
        /// <summary>
        /// Gets or sets the value indicating the interceptor that will handle field getters and setters.
        /// </summary>
        IFieldInterceptor FieldInterceptor { get; set; }
    }
}
