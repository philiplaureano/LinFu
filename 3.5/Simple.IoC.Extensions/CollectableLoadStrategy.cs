using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders.Interfaces;

namespace Simple.IoC.Extensions
{
    public class CollectableLoadStrategy<T> : ILoadStrategy
        where T : class
    {
        private IList<T> _results;
        private IContainer _container;
        public CollectableLoadStrategy(IList<T> results, IContainer container)
        {
            _results = results;
            _container = container;
        }
        #region ILoadStrategy Members

        public void ProcessLoadedTypes(IContainer hostContainer, List<Type> loadedTypes)
        {
            if (_results == null)
                return;

            // Choose types that implement the given interface and
            // have the Collectable attribute defined
            var validTypes = from t in loadedTypes
                             let constructor = t.GetConstructor(new Type[0])
                             where !t.IsAbstract && t.IsClass
                             && constructor != null
                             && typeof(T).IsAssignableFrom(t)
                             select t;

            // Instantiate each one of the loaded types
            var instances = from t in validTypes
                            let attributes = t.GetCustomAttributes(typeof(CollectableAttribute), true)
                            where attributes != null && attributes.Length > 0
                            select (T)Activator.CreateInstance(t);


            var instanceList = instances.ToList();

            if (instanceList.Count == 0)
                return;

            foreach (var instance in instanceList)
            {
                var initializer = instance as IInitialize;
                if (initializer != null && _container != null)
                {
                    initializer.Initialize(_container);
                }
                
                _results.Add(instance);
            }
        }

        #endregion
    }
}
