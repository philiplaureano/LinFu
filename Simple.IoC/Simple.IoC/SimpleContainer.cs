using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public class SimpleContainer : IContainer
    {
        private readonly Dictionary<Type, object> _factories = new Dictionary<Type, object>();
        private readonly List<IPropertyInjector> _propertyInjectors = new List<IPropertyInjector>();
        private readonly List<ITypeInjector> _injectors = new List<ITypeInjector>();
        private readonly List<ITypeSurrogate> _surrogates = new List<ITypeSurrogate>();
        public SimpleContainer()
        {
            _propertyInjectors.Add(new DefaultPropertyInjector());
        }      

        #region IContainer Members
        public void AddFactory<T>(IFactory<T> factory)
        {
            _factories[typeof (T)] = factory;
        }
        public void AddFactory(Type itemType, IFactory factory)
        {
#if DEBUG
            Console.WriteLine("Adding Factory for type '{0}'", itemType.FullName);
#endif
            _factories[itemType] = factory;
        }
        public virtual T GetService<T>() where T : class
        {
            Type serviceType = typeof(T);
            if (!_factories.ContainsKey(serviceType))
                return null;

            T result = CreateInstance<T>();

            if (result == null && _surrogates.Count > 0)
            {
                // Find a surrogate for the given type
                foreach (ITypeSurrogate surrogate in _surrogates)
                {
                    if (surrogate == null)
                        continue;

                    if (!surrogate.CanSurrogate(serviceType))
                        continue;

                    result = surrogate.ProvideSurrogate(serviceType) as T;
                }
            }

            if (result == null)
                throw new ServiceNotFoundException(serviceType);

            if (TypeInjectors.Count == 0)
                return result;

            foreach (ITypeInjector currentInjector in TypeInjectors)
            {
                object currentResult = null;
                // Allow third-party users to intercept instances
                // returned from this container            
                if (!typeof(ITypeInjector).IsAssignableFrom(serviceType) && currentInjector.CanInject(serviceType, result))
                    currentResult = currentInjector.Inject(serviceType, result);

                // Make sure that there is always a valid reference returned
                if (currentResult == null)
                    continue;
                
                result = (T)currentResult;
            }

            return result;
        }
        public virtual IList<ITypeInjector> TypeInjectors
        {
            get { return _injectors; }
        }
        public virtual IList<ITypeSurrogate> TypeSurrogates
        {
            get { return _surrogates; }
        }
        private T CreateInstance<T>() where T : class
        {
            Type serviceType = typeof (T);
            // Retrieve the factory
            object factoryInstance = _factories[serviceType];
            IFactory<T> factory = factoryInstance as IFactory<T>;
            T result = null;

            // Instantiate the object
            if (factory != null)
                result = factory.CreateInstance(this);

            // Use the non-generic IFactory instance if it doesn't work
            if (factory == null && factoryInstance is IFactory)
            {
                IFactory otherFactory = factoryInstance as IFactory;
                result = otherFactory.CreateInstance(this) as T;
            }
            
            if (result == null)
                return null;

            if (_propertyInjectors.Count == 0)
                return result;

            // Allow external clients to inject
            // properties into the objects as they see fit
            foreach (IPropertyInjector propertyInjector in _propertyInjectors)
            {
                if (propertyInjector == null || !propertyInjector.CanInject(result, this))
                    continue;

                propertyInjector.InjectProperties(result, this);
            }

            return result;
        }

        public void AddService<T>(T serviceInstance)
        {
            Type serviceType = typeof (T);
            _factories[serviceType] = new InstanceFactory<T>(serviceInstance);
        }                

        public IList<IPropertyInjector> PropertyInjectors
        {
            get { return _propertyInjectors; }
        }

        #endregion
    }
}
