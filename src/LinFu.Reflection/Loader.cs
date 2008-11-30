using System;
using System.Collections.Generic;
using System.Linq;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a generic loader class that can
    /// load multiple <see cref="Action{T}"/> delegates from multiple files and
    /// apply them to a particular <typeparamref name="TTarget"/> instance.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public class Loader<TTarget> : ILoader<TTarget>
    {
        private readonly List<Action<TTarget>> _actions = new List<Action<TTarget>>();
        private readonly List<Action<ILoader<TTarget>>> _loaderActions = new List<Action<ILoader<TTarget>>>();
        private readonly List<IActionLoader<TTarget, string>> _loaders = new List<IActionLoader<TTarget, string>>();

        private readonly AssemblyTargetLoader<ILoader<TTarget>> _pluginLoader = new AssemblyTargetLoader<ILoader<TTarget>>();
        private readonly List<ILoaderPlugin<TTarget>> _plugins = new List<ILoaderPlugin<TTarget>>();
        private readonly HashSet<string> _loadedFiles = new HashSet<string>();
        /// <summary>
        /// Initializes the target with the default settings.
        /// </summary>
        public Loader()
        {
            DirectoryLister = new DefaultDirectoryLister();

            // Make sure that loader plugins are loaded
            // on every LoadDirectory() call
            var pluginTypeLoader = new PluginLoader<TTarget, LoaderPluginAttribute>();
            _pluginLoader.TypeLoaders.Add(pluginTypeLoader);
        }

        /// <summary>
        /// The list of actions that will execute
        /// every time a target instance is configured.
        /// </summary>
        public IList<Action<TTarget>> QueuedActions
        {
            get { return _actions; }
        }

        /// <summary>
        /// The custom list of actions that will be
        /// performed prior to the beginning of the first load operation.
        /// </summary>
        /// <remarks>
        /// These actions will be performed only once per reset.
        /// </remarks>
        public IList<Action<ILoader<TTarget>>> CustomLoaderActions
        {
            get { return _loaderActions; }
        }

        /// <summary>
        /// The list of <see cref="ILoaderPlugin{TTarget}"/>
        /// instances that will be used to
        /// signal the beginning and end of the
        /// load sequence.
        /// </summary>
        public IList<ILoaderPlugin<TTarget>> Plugins
        {
            get { return _plugins; }
        }

        /// <summary>
        /// The list of <see cref="IActionLoader{TTarget, TInput}"/>
        /// instances responsible for configuring the target instance.
        /// </summary>
        public IList<IActionLoader<TTarget, string>> FileLoaders
        {
            get { return _loaders; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IDirectoryListing"/> instance 
        /// responsible for returning a list of filenames
        /// to the loader for processing.
        /// </summary>
        public IDirectoryListing DirectoryLister { get; set; }


        /// <summary>
        /// Loads the container with the files listed in 
        /// the target <paramref name="directory"/> that match 
        /// the given <paramref name="filespec">file pattern</paramref>.
        /// </summary>
        /// <param name="directory">The full path of the location to scan.</param>
        /// <param name="filespec">The wildcard file pattern string to use when specifying the target files.</param>
        public void LoadDirectory(string directory, string filespec)
        {
            // Determine which files exist
            IEnumerable<string> files = DirectoryLister.GetFiles(directory, filespec);

            foreach (string currentFile in files)
            {
                // Make sure the file is loaded only once
                if (_loadedFiles.Contains(currentFile))
                    continue;

                // HACK: Manually load any loader plugins
                // into the loader
                if (_pluginLoader.CanLoad(currentFile))
                {
                    // Immediately execute any custom loader actions
                    // embedded in the file itself
                    IEnumerable<Action<ILoader<TTarget>>> customActions = _pluginLoader.Load(currentFile);
                    foreach (var customAction in customActions)
                    {
                        customAction(this);
                    }
                }

                Load(currentFile);
                _loadedFiles.Add(currentFile);
            }
        }

        /// <summary>
        /// Loads the current configuration into the <paramref name="target"/>
        /// instance.
        /// </summary>
        /// <param name="target"></param>
        public void LoadInto(TTarget target)
        {
            // Abort the load if the container
            // is invalid
            if (ReferenceEquals(target, null))
                return;

            // Avoid duplicate actions by making 
            // sure that the loader executes
            // the list of custom actions once
            foreach (var customAction in CustomLoaderActions)
            {
                customAction(this);
            }

            CustomLoaderActions.Clear();


            // Signal the beginning of the load
            foreach (var plugin in Plugins)
            {
                if (plugin == null)
                    continue;

                plugin.BeginLoad(target);
            }

            // Configure the container
            foreach (var action in QueuedActions)
            {
                if (action == null)
                    continue;

                action(target);
            }

            // Signal the end of the load
            foreach (var plugin in Plugins)
            {
                if (plugin == null)
                    continue;

                plugin.EndLoad(target);
            }
        }

        /// <summary>
        /// Clears the currently loaded configuration
        /// and resets the loader back to its defaults.
        /// </summary>
        public void Reset()
        {
            _loaders.Clear();
            _plugins.Clear();
            _actions.Clear();
            _loaderActions.Clear();
            _loadedFiles.Clear();
        }
        

        /// <summary>
        /// Loads the <paramref name="currentFile">current file</paramref>
        /// using the list of associated <see cref="FileLoaders"/>.
        /// </summary>
        /// <param name="currentFile">The full path and filename being loaded.</param>
        private void Load(string currentFile)
        {
            foreach (var loader in FileLoaders)
            {
                if (loader == null || !loader.CanLoad(currentFile))
                    continue;

                IEnumerable<Action<TTarget>> actions = loader.Load(currentFile);
                if (actions.Count() == 0)
                    continue;

                _actions.AddRange(actions);
            }
        }
    }
}