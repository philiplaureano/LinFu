using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Marks a target type as an <see cref="IPreProcessor"/>
    /// instance that can be injected into a
    /// <see cref="IServiceContainer"/> instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PreprocessorAttribute : Attribute
    {
    }
}
