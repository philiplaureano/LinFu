using System;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a fluent class that creates
    /// a method that initializes a <typeparamref name="TService"/>
    /// instance.
    /// </summary>
    /// <typeparam name="TService">The service type being instantiated.</typeparam>
    internal class PropertyInjectionLambda<TService> : IPropertyInjectionLambda<TService>
    {
        private readonly ActionContext<TService> _context;

        /// <summary>
        /// Initializes the class with the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that will be associated with the target container.</param>
        internal PropertyInjectionLambda(ActionContext<TService> context)
        {
            _context = context;
        }

        /// <summary>
        /// Initializes service instances with the given
        /// <paramref name="action"/>.
        /// </summary>
        /// <param name="action">An <see cref="Action{TService}"/> delegate that will be used to initialize newly created services.</param>
        public void With(Action<TService> action)
        {
            Action<IServiceContainer, TService> adapter =
                (container, service) => action(service);

            _context.Action = adapter;

            AddPostProcessor(_context);
        }

        /// <summary>
        /// Uses an action delegate to initialize a given service using
        /// the given <see cref="IServiceContainer"/> and <typeparamref name="TService"/>
        /// instances.
        /// </summary>
        /// <param name="action">An <see cref="Func{IServiceContainer, TService}"/> delegate that will be used to initialize newly created services.</param>
        public void With(Action<IServiceContainer, TService> action)
        {
            _context.Action = action;
            AddPostProcessor(_context);
        }

        /// <summary>
        /// Attaches the action associated with the <see cref="ActionContext{TService}"/>
        /// instance to the target container embedded within the <see cref="ActionContext{TService}"/>
        /// class itself.
        /// </summary>
        /// <param name="context">The context that will be associated with the target container.</param>
        private static void AddPostProcessor(ActionContext<TService> context)
        {
            var targetContainer = context.Container;
            var postProcessor = new ActionPostProcessor<TService>(context);
            targetContainer.PostProcessors.Add(postProcessor);
        }        
    }
}
