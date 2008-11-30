using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that provides the most basic information
    /// for executing a fluent command against a 
    /// <see cref="IServiceContainer"/> instance.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    internal abstract class BaseContext<TService>
    {
        /// <summary>
        /// The service type to be created.
        /// </summary>
        public Type ServiceType 
        { 
            get
            {
                return typeof (TService);
            }
        }

        /// <summary>
        /// The name of the service to be created.
        /// </summary>
        public string ServiceName
        {
            get; set;
        }

        /// <summary>
        /// The actual <see cref="IServiceContainer"/>
        /// that ultimately will hold the service instance.
        /// </summary>
        public IServiceContainer Container
        {
            get; set;
        }
    }
}