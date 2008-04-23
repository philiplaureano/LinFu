using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Simple.IoC.Loaders.Interfaces;

namespace Simple.IoC.Loaders
{
    public class LoadPluginStrategy : ILoadStrategy
    {
        private readonly ILoadStrategy _strategy;
        public LoadPluginStrategy() {}
        public LoadPluginStrategy(ILoadStrategy innerStrategy)
        {
            _strategy = innerStrategy;
        }
        #region ILoadStrategy Members

        public void ProcessLoadedTypes(IContainer hostContainer, List<Type> loadedTypes)
        {
            #region Scan for plugins
            List<IContainerPlugin> plugins = new List<IContainerPlugin>();
            foreach(Type current in loadedTypes)
            {
                if (current == null)
                    continue;

                // Each plugin must have a default constructor
                ConstructorInfo defaultConstructor = current.GetConstructor(new Type[0]);
                if (defaultConstructor == null)
                    continue;

                // Load any additional type injectors
                if (current.IsDefined(typeof(TypeInjectorAttribute), true))
                {
                    ITypeInjector typeInjector = Activator.CreateInstance(current) as ITypeInjector;
                    
                    if (typeInjector != null)
                        hostContainer.TypeInjectors.Add(typeInjector);
                    
                    continue;
                }

                // Load any additional type customizers
                if (current.IsDefined(typeof(CustomizerAttribute), true))
                {
                    ICustomizeInstance customizer = Activator.CreateInstance(current) as ICustomizeInstance;

                    if (customizer != null)
                        hostContainer.Customizers.Add(customizer);

                    continue;
                }


                if (!current.IsDefined(typeof(ContainerPluginAttribute), true))
                    continue;

                IContainerPlugin plugin = Activator.CreateInstance(current) as IContainerPlugin;
                if (plugin == null)
                    continue;

                plugins.Add(plugin);
            }
            #endregion                       
            
            // Signal the beginning of the load
            plugins.ForEach(delegate(IContainerPlugin currentPlugin)
                                {
                                    currentPlugin.BeginLoad(hostContainer);
                                });

            if (_strategy != null)
                _strategy.ProcessLoadedTypes(hostContainer, loadedTypes);
            
            // Signal the end of the load
            plugins.ForEach(delegate(IContainerPlugin currentPlugin)
                                {
                                    currentPlugin.EndLoad(hostContainer);
                                });
        }

        #endregion
    }
}
