using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Simple.IoC.Loaders
{
    public class CustomFactoryLoader : BaseFactoryLoader
    {
        protected override object CreateFactory(Type factoryType, Type implementingType, Type serviceType)
        {
            return Activator.CreateInstance(implementingType);
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
