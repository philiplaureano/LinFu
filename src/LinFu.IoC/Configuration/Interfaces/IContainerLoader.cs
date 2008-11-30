using System;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a loader that reads a file and converts it
    /// into an equivalent set of a set of <see cref="Action{T}"/>
    /// instances that can be applied to a particular
    /// instance of an <see cref="IServiceContainer"/> class.
    /// </summary>
    public interface IContainerLoader : IActionLoader<IServiceContainer, string>
    {
    }
}