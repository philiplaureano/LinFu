using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the <c>internal</c> context class that will be used to 
    /// incrementally build enough information to inject a specific
    /// <see cref="IFactory{T}"/> instance into a container.
    /// </summary>
    /// <typeparam name="TService">The service type to be created.</typeparam>
    internal class InjectionContext<TService> : BaseContext<TService>
    {
        /// <summary>
        /// The factory method that will be used to
        /// instantiate the actual <typeparamref name="TService"/>
        /// instance.
        /// </summary>
        public Func<IFactoryRequest, TService> FactoryMethod
        {
            get; set;
        }
    }
}
