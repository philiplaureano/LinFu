using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration.Loaders
{
    /// <summary>
    /// A class that automatically loads <see cref="IPreProcessor"/>
    /// instances and configures a loader to inject those postprocessors
    /// into a container upon initialization.
    /// </summary>
    internal class PreProcessorLoader : IActionLoader<IServiceContainer, Type>
    {
        /// <summary>
        /// Determines if the plugin loader can load the <paramref name="inputType"/>.
        /// </summary>
        /// <remarks>The target type must implement the <see cref="IPreProcessor"/> interface before it can be loaded into memory.</remarks>
        /// <param name="inputType">The target type that might contain the target instance.</param>
        /// <returns><c>true</c> if the type can be loaded; otherwise, it returns <c>false</c>.</returns>
        public bool CanLoad(Type inputType)
        {
            // The type must have a default constructor
            var defaultConstructor = inputType.GetConstructor(new Type[0]);
            if (defaultConstructor == null)
                return false;

            // It must have the PreprocessorAttribute defined
            object[] attributes = inputType.GetCustomAttributes(typeof(PreprocessorAttribute), true);
            IEnumerable<PreprocessorAttribute> attributeList = attributes.Cast<PreprocessorAttribute>();

            if (attributeList.Count() == 0)
                return false;

            return typeof(IPreProcessor).IsAssignableFrom(inputType);
        }

        /// <summary>
        /// Loads a set of <see cref="IPreProcessor"/> instances
        /// so that they can be loaded into a container upon initialization.
        /// </summary>
        /// <param name="inputType">The type that will be used to configure the target loader.</param>
        /// <returns>A set of <see cref="Action{TTarget}"/> instances. This cannot be <c>null</c>.</returns>
        public IEnumerable<Action<IServiceContainer>> Load(Type inputType)
        {
            var defaultResult = new Action<IServiceContainer>[0];

            // Create the postprocessor instance
            var instance = Activator.CreateInstance(inputType) as IPreProcessor;
            if (instance == null)
                return defaultResult;

            // Inject the postprocessor into any service containers
            // that will be configured by the current loader instance
            Action<IServiceContainer> assignPreprocessor =
                container => container.PreProcessors.Add(instance);

            return new[] { assignPreprocessor };
        }
    }
}
