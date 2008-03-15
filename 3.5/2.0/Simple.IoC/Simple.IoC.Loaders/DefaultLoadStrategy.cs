using System;
using System.Collections.Generic;
using System.Text;
using Simple.IoC.Loaders.Interfaces;

namespace Simple.IoC.Loaders
{
    internal class DefaultLoadStrategy : ILoadStrategy
    {
        private IContainer _container;
        private IFactoryLoader _factoryLoader = new AutoFactoryLoader();
        public DefaultLoadStrategy(IContainer container, IFactoryLoader loader)
        {
            _container = container;
            FactoryLoader = loader;
        }

        public IFactoryLoader FactoryLoader
        {
            get { return _factoryLoader; }
            set { _factoryLoader = value; }
        }

        #region ILoadStrategy Members

        public void ProcessLoadedTypes(IContainer container, List<Type> loadedTypes)
        {
            if (FactoryLoader == null)
                return;

            foreach (Type currentType in loadedTypes)
            {
                FactoryLoader.LoadFactory(_container, currentType);
            }
        }

        #endregion       
    }
}
