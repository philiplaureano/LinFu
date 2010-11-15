using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a class that can customize a <see cref="ILoader{IServiceContainer}"/> instance.
    /// </summary>
    public interface IConfigureContainerLoader
    {
        /// <summary>
        /// Customizes the given <paramref name="targetLoader"/> instance.
        /// </summary>
        /// <param name="targetLoader">The target loader that will be customized by the current <see cref="IConfigureContainerLoader"/></param> instance.
        /// <param name="typeLoaders">The list of type loaders that will store the resulting type loaders.</param>
        void AddTypeLoaders(ILoader<IServiceContainer> targetLoader, IList<IActionLoader<IServiceContainer, Type>> typeLoaders);
    }
}
