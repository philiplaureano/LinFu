using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Simple.IoC
{
    internal class AssemblyLoader : IAssemblyLoader 
    {
        #region IAssemblyLoader Members

        public System.Reflection.Assembly LoadAssembly(string assemblyFile)
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

        #endregion        
    }
}
