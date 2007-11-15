using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Simple.IoC.Loaders.Interfaces;

namespace Simple.IoC.Loaders
{
    public abstract class BaseLoader
    {
        private IContainer _container;
        protected IAssemblyLoader _assemblyLoader = new AssemblyLoader();
        protected ITypeLoader _typeLoader = new TypeLoader();
        private ILoadStrategy _loadStrategy;
        protected BaseLoader()
        {
        }
            
        public BaseLoader(IContainer container)
        {
            _container = container;
        }

        public BaseLoader(IContainer container, IAssemblyLoader loader)
        {
            _container = container;
            _assemblyLoader = loader;
        }

        public BaseLoader(IContainer container, IAssemblyLoader loader, ITypeLoader typeLoader)
        {
            _container = container;
            _assemblyLoader = loader;            
            _typeLoader = typeLoader;
        }

        public virtual void LoadDirectory(string directory, string fileSpec)
        {
            string[] assemblyFiles = Directory.GetFiles(Path.GetFullPath(directory), Path.GetFileName(fileSpec));

            // Load each assembly and search for types that
            // implement IFactory<T>
            List<Type> loadedTypes = new List<Type>();
            foreach (string assemblyFile in assemblyFiles)
            {
                Assembly currentAssembly = AssemblyLoader.LoadAssembly(assemblyFile);
                if (currentAssembly == null)
                    continue;

                Type[] currentTypes = TypeLoader.LoadTypes(currentAssembly);
                if (currentTypes == null || currentTypes.Length == 0)
                    continue;

                loadedTypes.AddRange(currentTypes);
            }
            
            if (_loadStrategy == null)
                return;
            
            _loadStrategy.ProcessLoadedTypes(_container, loadedTypes);
        }        

        public IAssemblyLoader AssemblyLoader
        {
            get { return _assemblyLoader; }
            set { _assemblyLoader = value; }
        }

        public ITypeLoader TypeLoader
        {
            get { return _typeLoader; }
            set { _typeLoader = value; }
        }

        public ILoadStrategy LoadStrategy
        {
            get { return _loadStrategy; }
            set { _loadStrategy = value; }
        }

        public IContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }
    }
}