using System;
using System.Collections.Generic;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A factory that creates Singletons. Each service that this factory creates will only be created once per concrete type.
    /// </summary>
    /// <typeparam name="T">The type of service to instantiate.</typeparam>
    public class SingletonFactory<T> : BaseFactory<T>
    {
        private static readonly Dictionary<object, T> _instances = new Dictionary<object, T>();
        private readonly Func<IFactoryRequest, T> _createInstance;

        private readonly object _lock = new object();

        /// <summary>
        /// Initializes the factory class using the <paramref name="createInstance"/>
        /// parameter as a factory delegate.
        /// </summary>
        /// <example>
        /// The following is an example of initializing a <c>SingletonFactory&lt;T&gt;</c>
        /// type:
        /// <code>
        ///     // Define the factory delegate
        ///     Func&lt;IFactoryRequest, ISomeService&gt; createService = container=>new SomeServiceImplementation();
        /// 
        ///     // Create the factory
        ///     var factory = new SingletonFactory&lt;ISomeService&gt;(createService);
        /// 
        ///     // Use the service instance
        ///     var service = factory.CreateInstance(null);
        ///     
        ///     // ...
        /// </code>
        /// </example>
        /// <param name="createInstance">The delegate that will be used to create each new service instance.</param>
        public SingletonFactory(Func<IFactoryRequest, T> createInstance)
        {
            _createInstance = createInstance;
        }

        /// <summary>
        /// A method that creates a service instance as a singleton.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>A service instance as a singleton.</returns>
        public override T CreateInstance(IFactoryRequest request)
        {
            var key = new { request.ServiceName, request.ServiceType, request.Container };

            if (_instances.ContainsKey(key))
                return _instances[key];
                        
            lock (_lock)
            {
                T result = _createInstance(request);
                if (result != null)
                {                    
                    _instances[key] = result;
                }
            }

            return _instances[key];
        }
    }
}
