using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a fluent class that creates
    /// a factory method that will be used
    /// in instantiating a specific service instance.
    /// </summary>
    /// <typeparam name="TService">The service type being instantiated.</typeparam>
    public interface IUsingLambda<TService>
    {
        /// <summary>
        /// Creates a service instance using the
        /// concrete <typeparamref name="TConcrete"/> type 
        /// as the implementation for the <typeparamref name="TService"/>
        /// type.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete implementation that implements <typeparamref name="TService"/>. This class must have a default constructor.</typeparam>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        IGenerateFactory<TService> Using<TConcrete>() where TConcrete : TService;

        /// <summary>
        /// Creates a service instance using the
        /// <paramref name="factoryMethod"/> to
        /// instantiate the service instance
        /// with a particular factory type.
        /// </summary>
        /// <seealso cref="IGenerateFactory{T}"/>
        /// <param name="factoryMethod">The factory method that will be used to instantiate the actual service instance.</param>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        IGenerateFactory<TService> Using(Func<IServiceContainer, object[], TService> factoryMethod);

        /// <summary>
        /// Creates a service instance using the
        /// <paramref name="factoryMethod"/> to
        /// instantiate the service instance
        /// with a particular factory type.
        /// </summary>
        /// <seealso cref="IGenerateFactory{T}"/>
        /// <param name="factoryMethod">The factory method that will be used to instantiate the actual service instance.</param>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        IGenerateFactory<TService> Using(Func<IServiceContainer, TService> factoryMethod);

        /// <summary>
        /// Creates a service instance using the
        /// <paramref name="factoryMethod"/> to
        /// instantiate the service instance
        /// with a particular factory type.
        /// </summary>
        /// <param name="factoryMethod">The factory method that will be used to instantiate the actual service instance.</param>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        IGenerateFactory<TService> Using(Func<TService> factoryMethod);
    }
}
