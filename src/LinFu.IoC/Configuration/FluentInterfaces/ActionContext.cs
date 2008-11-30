using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the <c>internal</c> context class that will be used to 
    /// incrementally build enough information to initialize
    /// a specific <typeparamref name="TService"/> type once
    /// that service has been instantiated.
    /// </summary>
    /// <typeparam name="TService">The service type to be created.</typeparam>
    internal class ActionContext<TService> : BaseContext<TService>
    {
        /// <summary>
        /// The action that will be performed on an <see cref="IServiceContainer"/>
        /// instance once the fluent command executes.
        /// </summary>
        public Action<IServiceContainer, TService> Action { get; set; }
    }
}