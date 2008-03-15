using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace LinFu.AOP.Interfaces
{
    public class BootStrapInitializer : IInitializer
    {
        private bool _initialized;
        private bool _loadInProgress;
        private object lockObject = new object();
        #region IInitializer Members

        public bool CanInitialize(Type targetType)
        {
            if (_loadInProgress)
                return false;

            return _initialized == false;
        }

        public void CatchError(Exception ex)
        {

        }

        public void Initialize(object target)
        {
            // Do nothing
        }

        public void InitializeType(Type targetType)
        {
            if (_initialized)
                return;

            _loadInProgress = true;
            LoadInitializers();
            _loadInProgress = false;

            _initialized = true;
        }

        private void LoadInitializers()
        {
            string location = typeof(IModifiableType).Assembly.Location;
            string targetPath = Path.GetDirectoryName(location) + "\\";
            lock (lockObject)
            {
                string[] files = Directory.GetFiles(targetPath, "*.dll");

                foreach (var filename in files)
                {
                    Assembly currentAssembly = LoadAssembly(filename);
                    if (currentAssembly == null)
                        continue;

                    Type[] loadedTypes = LoadTypes(currentAssembly);

                    var initializerTypes = (from t in loadedTypes
                                            where IsInitializer(t)
                                            && typeof(IInitializer).IsAssignableFrom(t)
                                            select t).ToList();

                    initializerTypes.ForEach(InjectInitializerType);
                }
            }
        }
        private void InjectInitializerType(Type currentType)
        {
            try
            {
                // Find the default constructor
                ConstructorInfo constructor = currentType.GetConstructor(new Type[0]);

                if (constructor == null)
                    return;

                IInitializer initializer = Activator.CreateInstance(currentType) as IInitializer;
                if (initializer == null)
                    return;

                // Initialize the initializer itself
                initializer.InitializeSelf();

                InstanceRegistry.AddInitializer(initializer);
            }
            catch (Exception ex)
            {
                // Do nothing
            }
        }
        private bool IsInitializer(Type targetType)
        {
            object[] attributes = targetType.GetCustomAttributes(typeof(InitializerAttribute), false);
            if (attributes == null || attributes.Length == 0)
                return false;

            return true;
        }
        private Assembly LoadAssembly(string assemblyFile)
        {
            Assembly currentAssembly = null;

            try
            {
                currentAssembly = Assembly.LoadFile(assemblyFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return currentAssembly;
        }
        public Type[] LoadTypes(Assembly assembly)
        {
            Type[] loadedTypes = null;
            try
            {
                loadedTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                loadedTypes = ex.Types;
            }
            if (loadedTypes == null)
                loadedTypes = new Type[0];

            return loadedTypes.Where(t => t != null).ToArray();
        }

        #endregion

        #region IInitializer Members

        public void InitializeSelf()
        {

        }

        #endregion
    }
}
