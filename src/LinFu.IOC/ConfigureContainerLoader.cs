using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Loaders;
using LinFu.IoC.Interceptors;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a class that can customize a <see cref="ILoader{IServiceContainer}"/> instance.
    /// </summary>
    public class ConfigureContainerLoader : IConfigureContainerLoader
    {
        /// <summary>
        /// Customizes the given <paramref name="targetLoader"/> instance.
        /// </summary>
        /// <param name="targetLoader">The target loader that will be customized by the current <see cref="IConfigureContainerLoader"/></param> instance.
        /// <param name="typeLoaders">The list of type loaders that will store the resulting type loaders.</param>
        public void AddTypeLoaders(ILoader<IServiceContainer> targetLoader, IList<IActionLoader<IServiceContainer, Type>> typeLoaders)
        {
            typeLoaders.Add(new FactoryAttributeLoader());
            typeLoaders.Add(new PreProcessorLoader());
            typeLoaders.Add(new PostProcessorLoader());            
            typeLoaders.Add(new InterceptorAttributeLoader(targetLoader));
            typeLoaders.Add(new ImplementsAttributeLoader());

            // Load any additional service loaders
            var serviceLoaders = new List<ServiceLoader>();
            serviceLoaders.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            foreach (var loader in serviceLoaders)
            {
                typeLoaders.Add(loader);
            }
        }
    }
}
