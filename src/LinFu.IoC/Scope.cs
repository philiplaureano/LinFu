using System;
using System.Collections.Generic;
using System.Threading;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a class that keeps track of all the disposable objects 
    /// created within a service container and disposes them when 
    /// the scope itself has been disposed.
    /// </summary>
    public class Scope : IScope, IPostProcessor, IInitialize
    {
        private readonly List<WeakReference> _disposables = new List<WeakReference>();
        private IServiceContainer _container;
        private bool _disposed;
        private int _threadId;

        #region IInitialize Members

        /// <summary>
        /// Inserts the scope into the target <paramref name="source">container</paramref>.
        /// </summary>
        /// <param name="source">The container that will hold the scope instance.</param>
        public void Initialize(IServiceContainer source)
        {
            lock (this)
            {
                _container = source;

                // Use the same thread ID as the service instantiation call
                _threadId = Thread.CurrentThread.ManagedThreadId;

                // Monitor the container for 
                // any IDisposable instances that need to be disposed
                _container.PostProcessors.Add(this);
            }
        }

        #endregion

        #region IPostProcessor Members

        /// <summary>
        /// Monitors the <see cref="IServiceContainer"/> for any services that are created and automatically disposes them
        /// once the <see cref="IScope"/> is disposed.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> that describes the service being instantiated.</param>
        public void PostProcess(IServiceRequestResult result)
        {
            if (_disposed)
                return;

            // Only handle requests from the same thread
            if (_threadId != Thread.CurrentThread.ManagedThreadId)
                return;

            // Ignore any nondisposable instances
            if (result.ActualResult == null || !(result.ActualResult is IDisposable))
                return;

            var disposable = result.ActualResult as IDisposable;
            var weakRef = new WeakReference(disposable);
            _disposables.Add(weakRef);
        }

        #endregion

        #region IScope Members

        /// <summary>
        /// Disposes the services that have been created while the scope has been active.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (_threadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException(
                    "The scope object can only be disposed from within the thread that created it.");

            // Dispose all child objects
            foreach (var item in _disposables)
            {
                if (item == null)
                    continue;

                var target = item.Target as IDisposable;
                if (target == null)
                    continue;

                target.Dispose();
            }

            _disposed = true;

            // Remove the scope from the target container
            _container.PostProcessors.Remove(this);
        }

        #endregion
    }
}