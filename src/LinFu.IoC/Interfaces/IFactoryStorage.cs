using System;
using System.Collections.Generic;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a class that stores factory instances.
    /// </summary>
    public interface IFactoryStorage
    {
        /// <summary>
        /// Determines which factories should be used
        /// for a particular service request.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <returns>A factory instance.</returns>
        IFactory GetFactory(IServiceInfo serviceInfo);

        /// <summary>
        /// Adds a <see cref="IFactory"/> to the current <see cref="IFactoryStorage"/> object.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        void AddFactory(IServiceInfo serviceInfo, IFactory factory);

        /// <summary>
        /// Determines whether or not a factory exists in storage.
        /// </summary>
        /// <param name="serviceInfo">The <see cref="IServiceInfo"/> object that describes the target factory.</param>
        /// <returns>Returns <c>true</c> if the factory exists; otherwise, it will return <c>false</c>.</returns>
        bool ContainsFactory(IServiceInfo serviceInfo);

        /// <summary>
        /// Gets a value indicating the list of <see cref="IServiceInfo"/> objects
        /// that describe each available factory in the current <see cref="IFactoryStorage"/>
        /// instance.
        /// </summary>
        IEnumerable<IServiceInfo> AvailableFactories { get; }
    }
}