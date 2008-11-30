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
    internal class UsingLambda<TService> : IUsingLambda<TService>
    {
        private readonly InjectionContext<TService> _context;

        /// <summary>
        /// Initializes the class using the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">the <c>internal</c> context class that will be used to 
        /// incrementally build enough information to inject a specific
        /// <see cref="IFactory{T}"/> instance into a container.</param>
        internal UsingLambda(InjectionContext<TService> context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a service instance using the
        /// concrete <typeparamref name="TConcrete"/> type 
        /// as the implementation for the <typeparamref name="TService"/>
        /// type.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete implementation that implements <typeparamref name="TService"/>. This class must have a default constructor.</typeparam>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        public IGenerateFactory<TService> Using<TConcrete>() where TConcrete : TService
        {
            // Let the container decide which constructor should be used at runtime
            Func<IFactoryRequest, TService> factoryMethod = request =>
                {
                    var container = (IServiceContainer)request.Container;
                    return (TService)container.AutoCreate(typeof(TConcrete), request.Arguments);
                };

            var context = new InjectionContext<TService>
                              {
                                  ServiceName = _context.ServiceName,
                                  Container = _context.Container,
                                  FactoryMethod = factoryMethod
                              };


            return new GenerateFactory<TService>(context);
        }

        /// <summary>
        /// Creates a service instance using the
        /// <paramref name="factoryMethod"/> to
        /// instantiate the service instance
        /// with a particular factory type.
        /// </summary>
        /// <seealso cref="IGenerateFactory{T}"/>
        /// <param name="factoryMethod">The factory method that will be used to instantiate the actual service instance.</param>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        public IGenerateFactory<TService> Using(Func<IServiceContainer, object[], TService> factoryMethod)
        {
            Func<IFactoryRequest, TService> adapter =
                (request) => factoryMethod((IServiceContainer)request.Container, request.Arguments);

            var context = new InjectionContext<TService>
            {
                Container = _context.Container,
                FactoryMethod = adapter,
                ServiceName = _context.ServiceName
            };

            return new GenerateFactory<TService>(context);
        }

        /// <summary>
        /// Creates a service instance using the
        /// <paramref name="factoryMethod"/> to
        /// instantiate the service instance
        /// with a particular factory type.
        /// </summary>
        /// <seealso cref="IGenerateFactory{T}"/>
        /// <param name="factoryMethod">The factory method that will be used to instantiate the actual service instance.</param>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        public IGenerateFactory<TService> Using(Func<IServiceContainer, TService> factoryMethod)
        {
            Func<IFactoryRequest, TService> adapter =
                request => factoryMethod((IServiceContainer)request.Container);

            var context = new InjectionContext<TService>
            {
                Container = _context.Container,
                FactoryMethod = adapter,
                ServiceName = _context.ServiceName
            };

            return new GenerateFactory<TService>(context);
        }

        /// <summary>
        /// Creates a service instance using the
        /// <paramref name="factoryMethod"/> to
        /// instantiate the service instance
        /// with a particular factory type.
        /// </summary>
        /// <param name="factoryMethod">The factory method that will be used to instantiate the actual service instance.</param>
        /// <returns>A non-null <see cref="IGenerateFactory{T}"/> instance that will be used to create a factory and add it to a specific container.</returns>
        public IGenerateFactory<TService> Using(Func<TService> factoryMethod)
        {
            Func<IServiceContainer, object[], TService> adapter = (container, arguments) => factoryMethod();
            return Using(adapter);
        }
    }
}
