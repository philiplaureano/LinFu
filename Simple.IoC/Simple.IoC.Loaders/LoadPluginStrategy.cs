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
                if (current == null || !current.IsDefined(typeof(ContainerPluginAttribute), true))
                    continue;

                // Each plugin must have a default constructor
                ConstructorInfo defaultConstructor = current.GetConstructor(new Type[0]);
                if (defaultConstructor == null)
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
