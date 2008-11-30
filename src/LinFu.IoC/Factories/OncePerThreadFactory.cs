using System;
using System.Collections.Generic;
using System.Threading;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A factory that creates service instances that are unique
    /// from within the same thread as the factory itself.
    /// </summary>
    /// <typeparam name="T">The type of service to instantiate.</typeparam>
    public class OncePerThreadFactory<T> : BaseFactory<T>
    {
        private static readonly Dictionary<int, T> _storage = new Dictionary<int, T>();
        private readonly Func<IFactoryRequest, T> _createInstance;

        /// <summary>
        /// Initializes the factory class using the <paramref name="createInstance"/>
        /// parameter as a factory delegate.
        /// </summary>
        /// <example>
        /// The following is an example of initializing a <c>OncePerThreadFactory&lt;T&gt;</c>
        /// type:
        /// <code>
        ///     // Define the factory delegate
        ///     Func&lt;IFactoryRequest, ISomeService&gt; createService = container=>new SomeServiceImplementation();
        /// 
        ///     // Create the factory
        ///     var factory = new OncePerThreadFactory&lt;ISomeService&gt;(createService);
        /// 
        ///     // Use the service instance
        ///     var service = factory.CreateInstance(null);
        ///     
        ///     // ...
        /// </code>
        /// </example>
        /// <param name="createInstance">The delegate that will be used to create each new service instance.</param>
        public OncePerThreadFactory(Func<IFactoryRequest, T> createInstance)
        {
            _createInstance = createInstance;
        }

        /// <summary>
        /// Creates the service instance using the given <see cref="IFactoryRequest"/>
        /// instance. Every service instance created from this factory will
        /// only be created once per thread.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>A a service instance as thread-wide singleton.</returns>
        public override T CreateInstance(IFactoryRequest request)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            T result = default(T);
            lock (_storage)
            {
                // Create the service instance only once
                if (!_storage.ContainsKey(threadId))
                    _storage[threadId] = _createInstance(request);

                result = _storage[threadId];
            }

            return result;
        }
    }
}