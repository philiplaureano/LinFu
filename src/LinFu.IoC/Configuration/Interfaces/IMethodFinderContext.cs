using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents the data associated with a <see cref="IMethodFinder{T}"/> search.
    /// </summary>
    public interface IMethodFinderContext
    {
        /// <summary>
        /// Gets or sets the value indicating the type arguments that will be passed to the target method.
        /// </summary>
        IEnumerable<Type> TypeArguments { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the list of arguments that will be passed to the target method.
        /// </summary>
        IEnumerable<object> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="System.Type">return type</see> of the target method.
        /// </summary>
        Type ReturnType { get; set; }
    }
}
