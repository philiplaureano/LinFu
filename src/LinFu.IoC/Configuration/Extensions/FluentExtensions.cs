using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// A class that adds fluent syntax support to <see cref="IServiceContainer"/>
    /// instances.
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        /// Injects a <typeparamref name="TService"/> type
        /// into a <paramref name="container"/> using the
        /// given <paramref name="serviceName"/>
        /// </summary>
        /// <typeparam name="TService">The type of service to inject.</typeparam>
        /// <param name="container">The container that will hold the actual service service instance.</param>
        /// <param name="serviceName">The name of the service to create.</param>
        /// <returns>A non-null <see cref="IUsingLambda{TService}"/> instance.</returns>
        public static IUsingLambda<TService> Inject<TService>(this IServiceContainer container, string serviceName)
        {
            var context = new InjectionContext<TService>
            {
                ServiceName = serviceName,
                Container = container
            };

            return new UsingLambda<TService>(context);
        }

        /// <summary>
        /// Injects a <typeparamref name="TService"/> type
        /// into a <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to inject.</typeparam>
        /// <param name="container">The container that will hold the actual service service instance.</param>
        /// <returns>A non-null <see cref="IUsingLambda{TService}"/> instance.</returns>
        public static IUsingLambda<TService> Inject<TService>(this IServiceContainer container)
        {
            var context = new InjectionContext<TService>
            {
                ServiceName = null,
                Container = container
            };

            return new UsingLambda<TService>(context);
        }

        /// <summary>
        /// Initializes services that match the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The service type to initialize.</typeparam>
        /// <param name="container">The container that will create the service itself.</param>        
        /// <returns>A <see cref="IPropertyInjectionLambda{T}"/> instance. This cannot be <c>null</c>.</returns>
        public static IPropertyInjectionLambda<TService> Initialize<TService>(this IServiceContainer container)
        {
            return container.Initialize<TService>(null);
        }

        /// <summary>
        /// Initializes services that match the given <paramref name="serviceName"/> and <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The service type to initialize.</typeparam>
        /// <param name="container">The container that will create the service itself.</param>
        /// <param name="serviceName">The name of the service to initialize.</param>
        /// <returns>A <see cref="IPropertyInjectionLambda{T}"/> instance. This cannot be <c>null</c>.</returns>
        public static IPropertyInjectionLambda<TService> Initialize<TService>(this IServiceContainer container, string serviceName)
        {
            var context = new ActionContext<TService>()
                              {
                                  ServiceName = serviceName,
                                  Container = container,
                              };

            return new PropertyInjectionLambda<TService>(context);
        }
        /// <summary>
        /// Converts a <see cref="Func{Type, IServiceContainer, TArgs, TService}"/>
        /// lambda into an equivalent <see cref="Func{Type, IContainer, TArgs, TService}"/>
        /// instance.
        /// </summary>
        /// <typeparam name="TService">The type of service to create.</typeparam>
        /// <param name="func">The lambda function to be converted.</param>
        /// <returns>The equivalent <see cref="Func{IFactoryRequest, TService}"/>
        /// that delegates its calls back to the <paramref name="func"/> lambda function.</returns>
        internal static Func<IFactoryRequest, TService> CreateAdapter<TService>(this Func<IFactoryRequest, TService> func)
        {
            Func<IFactoryRequest, TService> adapter = func;

            return adapter;
        }
    }
}
