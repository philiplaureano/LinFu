using System.Collections.Generic;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that initializes service instances that use
    /// the <see cref="IInitialize"/> interface.
    /// </summary>
    public class Initializer : IPostProcessor
    {
        private static readonly HashSet<IInitialize> _instances = new HashSet<IInitialize>();
        #region IPostProcessor Members

        /// <summary>
        /// Initializes every service that implements
        /// the <see cref="IInitialize"/> interface.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> instance that contains the service instance to be initialized.</param>
        public void PostProcess(IServiceRequestResult result)
        {            
            var originalResult = result.OriginalResult as IInitialize;
            var actualResult = result.ActualResult as IInitialize;
            var container = result.Container;

            // Initialize the original result, if possible
            Initialize(originalResult, container);

            // Initialize the actual result as well
            Initialize(actualResult, container);
        }

        /// <summary>
        /// Initializes the <paramref name="target"/> with the given <paramref name="container"/> instance.
        /// </summary>
        /// <param name="target">The target to initialize.</param>
        /// <param name="container">The container that will be introduced to the <see cref="IInitialize"/> instance.</param>        
        private void Initialize(IInitialize target, IServiceContainer container)
        {
            if (target == null)
                return;

            // Make sure that the target is initialized only once
            if (_instances.Contains(target))
                return;

            // Initialize the target
            target.Initialize(container);
            _instances.Add(target);
        }

        #endregion
    }
}