using System;
using System.Collections.Generic;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a generic interface for an abstract loader
    /// that can read configuration information from disk
    /// and apply it to a <typeparamref name="TTarget"/> instance.
    /// </summary>
    /// <typeparam name="TTarget">The class type being configured.</typeparam>
    public interface ILoader<TTarget>
    {
        /// <summary>
        /// The list of <see cref="ILoaderPlugin{TTarget}"/>
        /// instances that will be used to
        /// signal the beginning and end of the
        /// load sequence.
        /// </summary>
        IList<ILoaderPlugin<TTarget>> Plugins { get; }

        /// <summary>
        /// The list of <see cref="IActionLoader{TTarget, TInput}"/>
        /// instances responsible for configuring the <typeparamref name="TTarget"/> instance.
        /// </summary>
        IList<IActionLoader<TTarget, string>> FileLoaders { get; }

        /// <summary>
        /// Gets or sets the <see cref="IDirectoryListing"/> instance 
        /// responsible for returning a list of filenames
        /// to the loader for processing.
        /// </summary>
        IDirectoryListing DirectoryLister { get; set; }

        /// <summary>
        /// The custom list of actions that will be
        /// performed prior to the beginning of a load operation.
        /// </summary>
        IList<Action<ILoader<TTarget>>> CustomLoaderActions { get; }

        /// <summary>
        /// The list of actions that will execute
        /// every time a target instance is configured.
        /// </summary>
        IList<Action<TTarget>> QueuedActions { get; }

        /// <summary>
        /// Loads the configuration using the files listed in 
        /// the target <paramref name="directory"/> that match 
        /// the given <paramref name="filespec">file pattern</paramref>.
        /// </summary>
        /// <param name="directory">The full path of the location to scan.</param>
        /// <param name="filespec">The wildcard file pattern string to use when specifying the target files.</param>
        void LoadDirectory(string directory, string filespec);

        /// <summary>
        /// Configures the <paramref name="target"/> instance 
        /// using the configuration currently loaded from disk.
        /// </summary>
        /// <param name="target">The <typeparamref name="TTarget"/> instance to be configured.</param>
        void LoadInto(TTarget target);

        /// <summary>
        /// Clears the currently loaded configuration
        /// and resets the loader back to its defaults.
        /// </summary>
        void Reset();
    }
}