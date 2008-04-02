using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.MxClone.Interfaces
{
    public interface IAssemblyCache
    {
        bool Contains(string assemblyQualifiedName);
        Assembly Retrieve(string assemblyQualifiedName);
        void Store(string assemblyQualifiedName, Assembly assembly);
    }
}
