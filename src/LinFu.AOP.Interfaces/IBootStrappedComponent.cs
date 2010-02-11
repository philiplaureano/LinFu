using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that will be automatically initialized once the LinFu.AOP assembly is loaded into memory.
    /// </summary>
    public interface IBootStrappedComponent
    {
        /// <summary>
        /// Initializes the bootstrapped component.
        /// </summary>
        void Initialize();
    }
}
