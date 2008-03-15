using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Simple.IoC
{
    public class AssemblyLoader : IAssemblyLoader 
    {
        #region IAssemblyLoader Members

        public Assembly LoadAssembly(string assemblyFile)
        {
            Assembly currentAssembly = null;

            try
            {
                currentAssembly = LoadFile(assemblyFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return currentAssembly;
        }

        #endregion
   
        protected virtual Assembly LoadFile(string assemblyFile)
        {
            return Assembly.LoadFile(assemblyFile);
        }
    }
}
