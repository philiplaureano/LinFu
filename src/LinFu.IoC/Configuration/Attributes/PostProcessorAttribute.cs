using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Marks a target type as an <see cref="IPostProcessor"/>
    /// instance that can be injected into a
    /// <see cref="IServiceContainer"/> instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PostProcessorAttribute : Attribute
    {
    }
}