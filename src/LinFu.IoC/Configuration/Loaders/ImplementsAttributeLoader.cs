using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Loaders
{
    /// <summary>
    /// A loader class that scans a type for <see cref="ImplementsAttribute"/>
    /// attribute declarations and creates a factory for each corresponding 
    /// attribute instance.
    /// </summary>
    /// <seealso cref="IFactory"/>
    public class ImplementsAttributeLoader : ServiceLoader
    {
        /// <summary>
        /// Gets the <see cref="IImplementationInfo"/> instances that describe the services that the given <paramref name="sourceType"/>
        /// can implement.
        /// </summary>
        /// <param name="sourceType">The source type that represents the implementing type.</param>
        /// <returns>A list of <see cref="IImplementationInfo"/> object instances that can be implemented by the given <paramref name="sourceType"/></returns>
        protected override IEnumerable<IImplementationInfo> GetImplementations(Type sourceType)
        {
            // Extract the Implements attribute from the source type
            ICustomAttributeProvider provider = sourceType;
            var attributes = provider.GetCustomAttributes(typeof(ImplementsAttribute), false);
            var attributeList = attributes.Cast<ImplementsAttribute>().ToList();

            var implementations = from attribute in attributeList
                                  where attribute != null
                                  select new ImplementsAttributeImplementationInfoAdapter(attribute, sourceType) as IImplementationInfo;

            return implementations;
        }
    }
}