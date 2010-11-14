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
    public class ConfigureContainerLoader : IConfigureContainerLoader
    {
        public void AddTypeLoaders(ILoader<IServiceContainer> targetLoader, IList<IActionLoader<IServiceContainer, Type>> typeLoaders)
        {
            typeLoaders.Add(new FactoryAttributeLoader());
            typeLoaders.Add(new ImplementsAttributeLoader());
            typeLoaders.Add(new PreProcessorLoader());
            typeLoaders.Add(new PostProcessorLoader());
            typeLoaders.Add(new InterceptorAttributeLoader(targetLoader));
        }
    }
}
