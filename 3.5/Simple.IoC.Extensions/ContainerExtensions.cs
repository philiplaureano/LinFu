using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC.Extensions
{
    public static class ContainerExtensions
    {
        public static void AddNamedService<T>(this IContainer container, string serviceName,
            Func<T> factoryMethod, params Action<T>[] initializeActions)
            where T : class
        {
            var initialize = CombineActions<T>(initializeActions);

            var surrogate = new FactorySurrogate<T>(serviceName, factoryMethod, initialize);
            container.TypeSurrogates.Add(surrogate);
        }

        public static void AddCustomizationStep<T>(this IContainer container, string serviceName,
            params Action<T>[] initializeActions)
            where T : class
        {
            var initialize = CombineActions<T>(initializeActions);
            NamedCustomizer<T> customizer = new NamedCustomizer<T>(serviceName, initialize);
            container.Customizers.Add(customizer);
        }
        private static Action<T> CombineActions<T>(Action<T>[] initializeActions) where T : class
        {
            // Combine all the initialization steps
            // into a single action
            Action<T> initialize = delegate { };
            foreach (var action in initializeActions)
            {
                initialize += action;
            }
            return initialize;
        }
    }
}
