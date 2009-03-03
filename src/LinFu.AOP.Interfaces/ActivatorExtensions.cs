using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// An extension class that adds helper methods to the <see cref="IActivator{T}"/> interface.
    /// </summary>
    public static class ActivatorExtensions
    {
        /// <summary>
        /// Instantiates the <paramref name="targetType"/> with the given <paramref name="activator"/> and <paramref name="constructorArguments"/>.
        /// </summary>
        /// <param name="activator">The <see cref="IActivator{T}"/> instance that will be responsible for creating the <paramref name="targetType"/>.</param>
        /// <param name="targetType">The type to be created.</param>
        /// <param name="constructorArguments">The arguments that will be passed to the constructor.</param>
        /// <returns>An object reference that matches the given <paramref name="targetType"/>.</returns>
        public static object CreateInstance(this IActivator<IActivationContext> activator, Type targetType, object[] constructorArguments)
        {
            var context = new ActivationContext(targetType, constructorArguments);
            return activator.CreateInstance(context);
        }
        /// <summary>
        /// Instantiates the <paramref name="targetType"/> with the given <paramref name="activator"/> and <paramref name="constructorArguments"/>.
        /// </summary>
        /// <param name="activator">The <see cref="IActivator{T}"/> instance that will be responsible for creating the target type.</param>
        /// <param name="constructorArguments">The arguments that will be passed to the constructor.</param>
        /// <typeparam name="T">The target type that will be instantiated by the activator.</typeparam>
        /// <returns>An object reference that matches the given <paramref name="targetType"/>.</returns>
        public static T CreateInstance<T>(this IActivator<IActivationContext> activator, object[] constructorArguments)
        {
            return (T)activator.CreateInstance(typeof(T), constructorArguments);
        }
    }
}
