using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a method finder that uses a <see cref="IServiceContainer"/> instance
    /// during its method searches.
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    public interface IMethodFinderWithContainer<TMethod> : IMethodFinder<TMethod>
        where TMethod : MethodBase
    {
        /// <summary>
        /// Gets the value indicating the service container that will be used in the
        /// method search.
        /// </summary>
        IServiceContainer Container { get; }
    }
}
