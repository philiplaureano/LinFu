using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Factories;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a fluent class that allows
    /// users to create specific types of factories.
    /// </summary>
    /// <typeparam name="TService">The type of service being created.</typeparam>
    public interface IGenerateFactory<TService>
    {
        /// <summary>
        /// Creates a singleton factory.
        /// </summary>
        /// <seealso cref="SingletonFactory{T}"/>
        void AsSingleton();

        /// <summary>
        /// Creates a once per thread factory.
        /// </summary>
        /// <seealso cref="OncePerThreadFactory{T}"/>
        void OncePerThread();

        /// <summary>
        /// Creates a once per request factory.
        /// </summary>
        /// <seealso cref="OncePerRequestFactory{T}"/>
        void OncePerRequest();
    }
}
