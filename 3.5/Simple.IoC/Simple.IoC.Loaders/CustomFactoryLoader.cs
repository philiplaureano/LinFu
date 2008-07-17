using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Simple.IoC.Loaders
{
    public class CustomFactoryLoader : BaseFactoryLoader
    {
        private Dictionary<Type, object> _factoryInstances = new Dictionary<Type, object>();
        protected override object CreateFactory(Type factoryType, Type implementingType, Type serviceType)
        {
            object instance = null;

            // Create a new factory instance, if necessary
            if (!_factoryInstances.ContainsKey(implementingType))
            {
                // Make sure that each implementing type
                // only has one and only one instance
                instance = Activator.CreateInstance(implementingType);

                lock (_factoryInstances)
                {
                    _factoryInstances[implementingType] = instance;
                }
            }

            instance = _factoryInstances[implementingType];

            return result;
        }

        protected override bool CanLoad(Type loadedType)
        {
            if (!loadedType.IsDefined(typeof(FactoryAttribute), true))
                return false;

            if (loadedType.IsAbstract || loadedType.IsInterface || !loadedType.IsClass)
                return false;

            return true;
        }
        protected override void LoadAdditionalFactories(IContainer container, Type loadedType)
        {
            // Load nothing
        }
        protected override IEnumerable<Type> GetItemTypes(Type loadedType)
        {
            List<Type> results = new List<Type>();
            foreach (object currentAttribute in loadedType.GetCustomAttributes(true))
            {
                if (!(currentAttribute is FactoryAttribute))
                    continue;

                FactoryAttribute attribute = currentAttribute as FactoryAttribute;

                results.Add(attribute.InstanceType);
            }

            return results; 
        }
    }
}
