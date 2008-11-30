using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a fluent class that creates
    /// a method that initializes a <typeparamref name="TService"/>
    /// instance.
    /// </summary>
    /// <typeparam name="TService">The service type being instantiated.</typeparam>
    public interface IPropertyInjectionLambda<TService>
    {
        /// <summary>
        /// Initializes service instances with the given
        /// <paramref name="action"/>.
        /// </summary>
        /// <param name="action">An <see cref="Action{TService}"/> delegate that will be used to initialize newly created services.</param>
        void With(Action<TService> action);

        /// <summary>
        /// Uses an action delegate to initialize a given service using
        /// the given <see cref="IServiceContainer"/> and <typeparamref name="TService"/>
        /// instances.
        /// </summary>
        /// <param name="action">An <see cref="Func{IServiceContainer, TService}"/> delegate that will be used to initialize newly created services.</param>
        void With(Action<IServiceContainer, TService> action);
    }
}
