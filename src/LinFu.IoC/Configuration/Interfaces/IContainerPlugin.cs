using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents an alias interface used for backward compatibility with LinFu IoC 1.0
    /// </summary>
    public interface IContainerPlugin : ILoaderPlugin<IServiceContainer>
    {
    }
}
