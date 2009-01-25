using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that initializes service instances that use
    /// the <see cref="IInitialize{T}"/> interface.
    /// </summary>
    public class Initializer<T> : IPostProcessor
    {
        private static readonly HashSet<IInitialize<T>> _instances = new HashSet<IInitialize<T>>();
        private readonly Func<IServiceRequestResult, T> _getSource;
        #region IPostProcessor Members

        /// <summary>
        /// Initializes the class with the given <paramref name="getSource"/> delegate.
        /// </summary>
        /// <param name="getSource">The functor that will obtain the object instance that will be used to initialize a given service.</param>
        public Initializer(Func<IServiceRequestResult, T> getSource)
        {
            _getSource = getSource;
        }

        /// <summary>
        /// Initializes every service that implements
        /// the <see cref="IInitialize{T}"/> interface.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> instance that contains the service instance to be initialized.</param>
        public void PostProcess(IServiceRequestResult result)
        {
            var originalResult = result.OriginalResult as IInitialize<T>;
            var actualResult = result.ActualResult as IInitialize<T>;

            var source = _getSource(result);

            // Initialize the original result, if possible
            Initialize(originalResult, source);

            // Initialize the actual result
            Initialize(actualResult, source);
        }
        
        /// <summary>
        /// Initializes the <paramref name="target"/> with the given <paramref name="source"/> instance.
        /// </summary>
        /// <param name="target">The target to initialize.</param>
        /// <param name="source">The instance that will be introduced to the <see cref="IInitialize{T}"/> instance.</param>        
        private static void Initialize(IInitialize<T> target, T source)
        {
            if (target == null)
                return;

            // Make sure that the target is initialized only once
            if (_instances.Contains(target))
                return;

            // Initialize the target
            target.Initialize(source);
            _instances.Add(target);
        }

        #endregion
    }
}
