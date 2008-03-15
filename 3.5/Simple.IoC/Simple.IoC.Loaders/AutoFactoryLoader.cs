using System;
using System.Collections.Generic;
using System.Text;
using Simple.IoC.Factories;

namespace Simple.IoC.Loaders
{
    public class AutoFactoryLoader : BaseFactoryLoader
    {
        private static readonly Dictionary<LifecycleType, Type> 
            _typeMap = new Dictionary<LifecycleType, Type>();
        static AutoFactoryLoader()
        {
            _typeMap[LifecycleType.OncePerRequest] = typeof (OncePerRequestFactory<,>);
            _typeMap[LifecycleType.OncePerThread] = typeof (OncePerThreadFactory<,>);
            _typeMap[LifecycleType.Singleton] = typeof (SingletonFactory<,>);
        }
        protected override object CreateFactory(Type factoryInterfaceType, Type implementingType, Type serviceType)
        {
            object factoryInstance = null;
            object[] attributes = implementingType.GetCustomAttributes(typeof (ImplementsAttribute), true);
            foreach(object attribute in attributes)
            {
                ImplementsAttribute currentAttribute = attribute as ImplementsAttribute;
                if (currentAttribute == null)
                    continue;

                // Match the factory with the service type and instantiate it
                if (currentAttribute.ServiceType != serviceType)
                    continue;

                Type factoryGenericType = _typeMap[currentAttribute.LifeCycleType];
                Type factoryType = factoryGenericType.MakeGenericType(serviceType, implementingType);
                factoryInstance = Activator.CreateInstance(factoryType);
                break;
            }

            return factoryInstance;
        }

        public override void LoadFactory(IContainer container, Type loadedType)
        {
            // Load any custom factories associated with the assembly
            CustomFactoryLoader factoryLoader = new CustomFactoryLoader();
            factoryLoader.LoadFactory(container, loadedType);

            // Load any named services associated with the assembly
            NamedFactoryLoader namedFactoryLoader = new NamedFactoryLoader();
            namedFactoryLoader.LoadFactory(container, loadedType);

            base.LoadFactory(container, loadedType);
        }
        protected override bool CanLoad(Type loadedType)
        {
            if (loadedType.IsAbstract || loadedType.IsInterface || !loadedType.IsClass)
                return false;

            if (!loadedType.IsDefined(typeof(ImplementsAttribute), true))
                return false;

            ImplementsAttribute attribute = (ImplementsAttribute)loadedType.GetCustomAttributes(typeof(ImplementsAttribute), true)[0];

            if (!ShouldLoad(attribute.ServiceName, loadedType))
                return false;

            return true;
        }
        protected virtual bool ShouldLoad(string serviceName, Type loadedType)
        {
            // Load only nameless services
            if (!string.IsNullOrEmpty(serviceName))
                return false;

            return true;
        }
        protected override IEnumerable<Type> GetItemTypes(Type currentType)
        {
            List<Type> results = new List<Type>();
            foreach (object currentAttribute in currentType.GetCustomAttributes(true))
            {
                if (!(currentAttribute is ImplementsAttribute))
                    continue;

                ImplementsAttribute attribute = currentAttribute as ImplementsAttribute;

                results.Add(attribute.ServiceType);
            }

            return results;
        }
    }
}
