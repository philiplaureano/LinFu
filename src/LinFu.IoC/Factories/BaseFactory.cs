using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A factory base class that combines both the IFactory and
    /// the IFactory&lt;T&gt; interfaces into a single class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseFactory<T> : IFactory<T>, IFactory
    {
        /// <summary>
        /// Creates a service instance using the given container.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        object IFactory.CreateInstance(IFactoryRequest request)
        {
            return CreateInstance(request);
        }

        /// <summary>
        /// Creates a service instance using the given container.
        /// </summary>
        /// <remarks>
        /// <see cref="IFactory"/> developers can inherit from this class
        /// instead of having to write their own custom factories
        /// from scratch. This should cut down on some of the boilerplate
        /// code necessary to get a factory class up and running.
        /// </remarks>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        public abstract T CreateInstance(IFactoryRequest request);
    }
}
