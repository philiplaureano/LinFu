using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents an <see cref="IPreProcessor"/> type that processes multiple <see cref="IPreProcessor"/> instances at once.
    /// </summary>
    internal class CompositePreProcessor : IPreProcessor
    {
        private readonly IEnumerable<IPreProcessor> _preProcessors;

        /// <summary>
        /// Initializes the type using the given <paramref name="preProcessors"/>.
        /// </summary>
        /// <param name="preProcessors">The list of <see cref="IPreProcessor"/> instances that will be handled by this type.</param>
        public CompositePreProcessor(IEnumerable<IPreProcessor> preProcessors)
        {
            _preProcessors = preProcessors;
        }

        /// <summary>
        /// A method that passes every request result made
        /// to the list of preprocessors.
        /// </summary>
        /// <param name="request">The parameter that describes the context of the service request.</param>
        public void Preprocess(IServiceRequest request)
        {
            foreach (var preprocessor in _preProcessors)
            {
                preprocessor.Preprocess(request);
            }
        }
    }
}
