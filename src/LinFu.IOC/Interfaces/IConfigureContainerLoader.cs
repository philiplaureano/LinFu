using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.IoC.Interfaces
{
    public interface IConfigureContainerLoader
    {
        void AddTypeLoaders(ILoader<IServiceContainer> targetLoader, IList<IActionLoader<IServiceContainer, Type>> typeLoaders);
    }
}
