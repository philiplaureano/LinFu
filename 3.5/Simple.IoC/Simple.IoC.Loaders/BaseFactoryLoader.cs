using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Simple.IoC.Loaders
{
    public abstract class BaseFactoryLoader : IFactoryLoader 
    {
        #region IFactoryLoader Members

        public virtual void LoadFactory(IContainer container, Type loadedType)
        {
            if (!CanLoad(loadedType))
                return;

            IEnumerable<Type> itemTypes = GetItemTypes(loadedType);

			foreach(Type itemType in itemTypes)
			{
	            Debug.Assert(itemType != null);
	            if (itemType == null)
	                continue;

	            Type factoryType = GetFactoryType(itemType);

	            // Create the factory itself
	            object instance = CreateFactory(factoryType, loadedType, itemType);
	            object factoryInstance = Cast(factoryType, instance);
			    
	            if (factoryInstance == null)
	                continue;

	            // Add the object factory to the container
	            MethodInfo addFactoryDefinition =
	                typeof(BaseFactoryLoader).GetMethod("AddFactory", BindingFlags.NonPublic | BindingFlags.Static);

	            Debug.Assert(addFactoryDefinition.IsGenericMethodDefinition);

	            MethodInfo addFactory = addFactoryDefinition.MakeGenericMethod(itemType);
	            addFactory.Invoke(null, new object[] { factoryInstance, container });
            }
        }

        #endregion

        protected abstract object CreateFactory(Type factoryType, Type currentType, Type serviceType);
        protected abstract bool CanLoad(Type loadedType);
        protected virtual Type GetFactoryType(Type currentType)
        {
            return typeof (IFactory<>).MakeGenericType(currentType);
        }
        protected abstract IEnumerable<Type> GetItemTypes(Type currentType);

        #region Private Members
        private static void AddFactory<T>(object factoryInstance, IContainer container)
        {
            IFactory<T> factory = factoryInstance as IFactory<T>;
            container.AddFactory(factory);
        }
        private static object Cast(Type targetType, object instance)
        {
            MethodInfo castMethodDefinition = typeof(BaseFactoryLoader).GetMethod("CastAs", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(castMethodDefinition != null);

            MethodInfo castMethod = castMethodDefinition.MakeGenericMethod(targetType);
            return castMethod.Invoke(null, new object[] { instance });
        }
        private static object CastAs<T>(object instance)
            where T : class
        {
            return instance as T;
        }
        #endregion
    }
}
