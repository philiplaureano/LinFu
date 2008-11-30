using System;
using System.Collections.Generic;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a class that can configure 
    /// a target of type <typeparamref name="TTarget"/>
    /// using an input type of <typeparamref name="TInput"/>.
    /// </summary>
    /// <typeparam name="TTarget">The target type to configure.</typeparam>
    /// <typeparam name="TInput">The input that will be used to configure the target.</typeparam>
    public interface IActionLoader<TTarget, TInput>
    {
        /// <summary>
        /// Generates a set of <see cref="Action{T}"/> instances
        /// using the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input that will be used to configure the target.</param>
        /// <returns>A set of <see cref="Action{TTarget}"/> instances. This cannot be <c>null</c>.</returns>
        IEnumerable<Action<TTarget>> Load(TInput input);

        /// <summary>
        /// Determines if the PluginLoader can load the <paramref name="inputType"/>.
        /// </summary>
        /// <param name="inputType">The target type that might contain the target <typeparamref name="TAttribute"/> instance.</param>
        /// <returns><c>true</c> if the type can be loaded; otherwise, it returns <c>false</c>.</returns>
        bool CanLoad(TInput inputType);
    }
}