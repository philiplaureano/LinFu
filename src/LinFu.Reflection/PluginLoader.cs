using System;
using System.Collections.Generic;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a loader class that can load
    /// <see cref="ILoaderPlugin{TTarget}"/> instances
    /// marked with a particular <typeparamref name="TAttribute"/>
    /// type.
    /// </summary>
    /// <typeparam name="TTarget">The target type being configured.</typeparam>
    /// <typeparam name="TAttribute">The attribute type that marks a type as a plugin type.</typeparam>
    public class PluginLoader<TTarget, TAttribute> : BasePluginLoader<TTarget, TAttribute>
        where TAttribute : Attribute
    {
        /// <summary>
        /// Determines if the plugin loader can load the <paramref name="inputType"/>.
        /// </summary>
        /// <remarks>The target type must implement <see cref="ILoaderPlugin{TTarget}"/> before it can be loaded.</remarks>
        /// <param name="inputType">The target type that might contain the target <typeparamref name="TAttribute"/> instance.</param>
        /// <returns><c>true</c> if the type can be loaded; otherwise, it returns <c>false</c>.</returns>
        public override bool CanLoad(Type inputType)
        {
            return base.CanLoad(inputType) &&
                   typeof(ILoaderPlugin<TTarget>).IsAssignableFrom(inputType);
        }

        /// <summary>
        /// Loads a set of actions from a <see cref="System.Type"/>
        /// instance.
        /// </summary>
        /// <param name="input">The target type to scan.</param>
        /// <returns>A set of actions which will be applied to the target <see cref="ILoader{T}"/> instance.</returns>
        public override IEnumerable<Action<ILoader<TTarget>>> Load(Type input)
        {
            // Create the plugin instance
            var plugin = Activator.CreateInstance(input) as ILoaderPlugin<TTarget>;

            if (plugin == null)
                return new Action<ILoader<TTarget>>[0];

            // Assign it to the target loader
            Action<ILoader<TTarget>> result =
                loader =>
                {
                    // If possible, initialize the plugin
                    // with the loader
                    if (plugin is IInitialize<ILoader<TTarget>>)
                    {
                        var target = plugin as IInitialize<ILoader<TTarget>>;
                        target.Initialize(loader);
                    }

                    loader.Plugins.Add(plugin);
                };

            // Package it into an array and return the result
            return new[] { result };
        }
    }
}