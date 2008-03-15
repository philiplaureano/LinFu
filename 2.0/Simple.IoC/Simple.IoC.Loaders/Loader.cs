using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Simple.IoC.Loaders
{
    public class Loader : BaseLoader
    {
        private IFactoryLoader _factoryLoader = new AutoFactoryLoader();
        public Loader(IContainer container) : base(container)
        {
        }
        public Loader(IContainer container, IAssemblyLoader loader) : base(container, loader)
        {
        }
        public Loader(IContainer container, IAssemblyLoader loader, ITypeLoader typeLoader) : base(container, loader, typeLoader)
        {
        }
        public Loader(IContainer container, IAssemblyLoader loader, ITypeLoader typeLoader, IFactoryLoader factoryLoader) : base(container, loader, typeLoader)
        {            
            
        }

        public override void LoadDirectory(string directory, string fileSpec)
        {
            if (LoadStrategy == null)
                LoadStrategy = new LoadPluginStrategy(new DefaultLoadStrategy(Container, _factoryLoader));

            base.LoadDirectory(directory, fileSpec);
        }
        public IFactoryLoader FactoryLoader
        {
            get
            {
                return _factoryLoader;
            }
            set
            {
                _factoryLoader = value;
            }
        }
    }
}
