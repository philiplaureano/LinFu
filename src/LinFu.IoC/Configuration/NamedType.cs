using System;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a named type.
    /// </summary>
    public class NamedType : INamedType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedType"/> class.
        /// </summary>
        public NamedType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedType"/> class.
        /// </summary>
        /// <param name="currentType">The current type.</param>
        public NamedType(Type currentType)
        {
            Type = currentType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedType"/> class.
        /// </summary>
        /// <param name="parameter">The target parameter.</param>
        public NamedType(ParameterInfo parameter)
        {
            Name = parameter.Name;
            Type = parameter.ParameterType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedType"/> class.
        /// </summary>
        /// <param name="property">The target property.</param>
        public NamedType(PropertyInfo property)
        {
            Name = property.Name;
            Type = property.PropertyType;
        }


        /// <summary>
        /// Gets or sets a value indicating the name that will be associated with the current type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the current target type.
        /// </summary>
        public Type Type { get; set; }
    }
}