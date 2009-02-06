using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents an <see cref="IPostProcessor"/> type that processes multiple <see cref="IPostProcessor"/> instances at once.
    /// </summary>
    internal class CompositePostProcessor : IPostProcessor
    {
        private readonly IEnumerable<IPostProcessor> _postProcessors;

        /// <summary>
        /// Initializes the type using the given <paramref name="postProcessors"/>.
        /// </summary>
        /// <param name="postProcessors">The list of <see cref="IPostProcessor"/> instances that will be handled by this type.</param>
        internal CompositePostProcessor(IEnumerable<IPostProcessor> postProcessors)
        {
            _postProcessors = postProcessors;
        }

        /// <summary>
        /// A method that passes every request result made
        /// to the list of postprocessors.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> instance that describes the result of the service request.</param>        
        /// <returns>A <see cref="IServiceRequestResult"/> representing the results returned as a result of the postprocessors.</returns>
        public void PostProcess(IServiceRequestResult result)
        {
            // Let each postprocessor inspect 
            // the results and/or modify the 
            // returned object instance
            var postprocessors = _postProcessors.ToArray();
            foreach (var postProcessor in postprocessors)
            {
                if (postProcessor == null)
                    continue;

                postProcessor.PostProcess(result);
            }
        }
    }
}
