using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// An <see cref="IMemberInjectionFilter{PropertyInfo}"/> implementation
    /// that automatically selects properties whose property types
    /// currently exist in the target container.
    /// </summary>
    public class PropertyInjectionFilter : BaseMemberInjectionFilter<PropertyInfo>
    {
        /// <summary>
        /// Determines which members should be selected from the <paramref name="targetType"/>
        /// using the <paramref name="container"/>
        /// </summary>
        /// <param name="targetType">The target type that will supply the list of members that will be filtered.</param>
        /// <param name="container">The target container.</param>
        /// <returns>A list of <see cref="PropertyInfo"/> objects that pass the filter description.</returns>
        protected override IEnumerable<PropertyInfo> GetMembers(Type targetType, IServiceContainer container)
        {
            var results = from p in targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                          let propertyType = p.PropertyType
                          let isServiceArray = propertyType.ExistsAsServiceArray()
                          let isCompatible = isServiceArray(container) || container.Contains(propertyType)
                          where p.CanWrite && isCompatible
                          select p;

            return results;
        }
    }
}
