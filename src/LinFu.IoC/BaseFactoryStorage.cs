using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents the default base implementation of the <see cref="IFactoryStorage"/> class.
    /// </summary>
    public abstract class BaseFactoryStorage : IFactoryStorage
    {
        private readonly object _lock = new object();
        private readonly Dictionary<IServiceInfo, IFactory> _entries = new Dictionary<IServiceInfo, IFactory>();

        /// <summary>
        /// Determines which factories should be used
        /// for a particular service request.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <returns>A factory instance.</returns>
        public virtual IFactory GetFactory(IServiceInfo serviceInfo)
        {
            if (_entries.ContainsKey(serviceInfo))
                return _entries[serviceInfo];

            return null;
        }

        /// <summary>
        /// Adds a <see cref="IFactory"/> to the current <see cref="IFactoryStorage"/> object.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public virtual void AddFactory(IServiceInfo serviceInfo, IFactory factory)
        {
            lock (_lock)
            {               
                _entries[serviceInfo] = factory;
            }
        }

        /// <summary>
        /// Determines whether or not a factory exists in storage.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <returns>Returns <c>true</c> if the factory exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool ContainsFactory(IServiceInfo serviceInfo)
        {            
            return _entries.ContainsKey(serviceInfo);
        }

        /// <summary>
        /// Gets a value indicating the list of <see cref="IServiceInfo"/> objects
        /// that describe each available factory in the current <see cref="IFactoryStorage"/>
        /// instance.
        /// </summary>
        public virtual IEnumerable<IServiceInfo> AvailableFactories
        {
            get
            {                
                return _entries.Keys;
            }
        }
    }
}
