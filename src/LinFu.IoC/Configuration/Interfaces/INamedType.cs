using System;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a named type.
    /// </summary>
    public interface INamedType
    {
        /// <summary>
        /// Gets or sets a value indicating the name that will be associated with the current type.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the current target type.
        /// </summary>
        Type Type { get; set; }
    }
}