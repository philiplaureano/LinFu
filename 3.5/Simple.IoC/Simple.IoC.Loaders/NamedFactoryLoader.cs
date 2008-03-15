using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Simple.IoC.Loaders
{
    public class NamedFactoryLoader : AutoFactoryLoader
    {
        protected override bool ShouldLoad(string serviceName, Type loadedType)
        {
            return !string.IsNullOrEmpty(serviceName);
        }
        protected override void InsertFactory(IContainer container, Type itemType, Type loadedType, object factoryInstance)
        {
            object[] attributes = loadedType.GetCustomAttributes(typeof(ImplementsAttribute), true);
            if (attributes == null || attributes.Length == 0)
                return;

            if (container.NamedFactoryStorage == null)
                return;

            ImplementsAttribute attribute = (ImplementsAttribute)attributes[0];
            string serviceName = attribute.ServiceName;

            // Add the named object factory to the container
            MethodInfo addFactoryDefinition =
                typeof(INamedFactoryStorage).GetMethod("Store", BindingFlags.Public | BindingFlags.Instance);

            Debug.Assert(addFactoryDefinition.IsGenericMethodDefinition);

            MethodInfo storeFactory = addFactoryDefinition.MakeGenericMethod(itemType);
            storeFactory.Invoke(container.NamedFactoryStorage, new object[] { serviceName, factoryInstance});
        }
    }
}
