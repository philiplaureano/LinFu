using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a postprocessor that will execute
    /// the action associated with the given <see cref="ActionContext{TService}"/>
    /// instance every time the target container returns a 
    /// service with particular service name and service type.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    internal class ActionPostProcessor<TService> : IPostProcessor
    {
        private readonly ActionContext<TService> _context;

        internal ActionPostProcessor(ActionContext<TService> context)
        {
            _context = context;
        }

        #region IPostProcessor Members

        public void PostProcess(IServiceRequestResult result)
        {
            // Ignore any null results
            if (result.ActualResult == null)
                return;

            // The service type must match the current service
            if (result.ServiceType != typeof (TService))
                return;

            // The service names must be equal
            if (result.ServiceName != _context.ServiceName)
                return;

            var service = (TService) result.ActualResult;
            IServiceContainer container = result.Container;
            Action<IServiceContainer, TService> action = _context.Action;

            // Execute the action associated with the
            // context
            action(container, service);
        }

        #endregion
    }
}