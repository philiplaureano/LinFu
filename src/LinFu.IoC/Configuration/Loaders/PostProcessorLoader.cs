using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration.Loaders
{
    /// <summary>
    ///     A class that automatically loads <see cref="IPostProcessor" />
    ///     instances and configures a loader to inject those postprocessors
    ///     into a container upon initialization.
    /// </summary>
    internal class PostProcessorLoader : IActionLoader<IServiceContainer, Type>
    {
        /// <summary>
        ///     Determines if the plugin loader can load the <paramref name="inputType" />.
        /// </summary>
        /// <remarks>
        ///     The target type must implement the <see cref="IPostProcessor" /> interface before it can be loaded into
        ///     memory.
        /// </remarks>
        /// <param name="inputType">The target type that might contain the target instance.</param>
        /// <returns><c>true</c> if the type can be loaded; otherwise, it returns <c>false</c>.</returns>
        public bool CanLoad(Type inputType)
        {
            try
            {
                // The type must have a default constructor
                var defaultConstructor = inputType.GetConstructor(new Type[0]);
                if (defaultConstructor == null)
                    return false;

                // It must have the PostProcessorAttribute defined
                var attributes = inputType.GetCustomAttributes(typeof(PostProcessorAttribute), true);
                var attributeList = attributes.Cast<PostProcessorAttribute>();

                if (attributeList.Count() == 0)
                    return false;

                return typeof(IPostProcessor).IsAssignableFrom(inputType);
            }
            catch (TypeInitializationException)
            {
                // Ignore the error
                return false;
            }
            catch (FileNotFoundException)
            {
                // Ignore the error
                return false;
            }
        }

        /// <summary>
        ///     Loads a set of <see cref="IPostProcessor" /> instances
        ///     so that they can be loaded into a container upon initialization.
        /// </summary>
        /// <param name="inputType">The type that will be used to configure the target loader.</param>
        /// <returns>A set of <see cref="Action{TTarget}" /> instances. This cannot be <c>null</c>.</returns>
        public IEnumerable<Action<IServiceContainer>> Load(Type inputType)
        {
            var defaultResult = new Action<IServiceContainer>[0];

            // Create the postprocessor instance
            var instance = Activator.CreateInstance(inputType) as IPostProcessor;
            if (instance == null)
                return defaultResult;

            // Inject the postprocessor into any service containers
            // that will be configured by the current loader instance
            Action<IServiceContainer> assignPostProcessor =
                container => container.PostProcessors.Add(instance);

            return new[] {assignPostProcessor};
        }
    }
}