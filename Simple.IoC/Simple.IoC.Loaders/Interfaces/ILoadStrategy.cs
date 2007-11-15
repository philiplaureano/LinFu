using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Loaders.Interfaces
{
    public interface ILoadStrategy
    {
        void ProcessLoadedTypes(IContainer hostContainer, List<Type> loadedTypes);
    }
}
