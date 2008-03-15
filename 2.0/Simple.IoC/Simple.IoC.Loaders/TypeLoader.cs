using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Simple.IoC
{
    internal class TypeLoader : ITypeLoader 
    {
        #region ITypeLoader Members

        public Type[] LoadTypes(System.Reflection.Assembly assembly)
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
            return loadedTypes;
        }

        #endregion
    }
}
