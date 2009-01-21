using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the data associated with a <see cref="IMethodFinder{T}"/> search.
    /// </summary>
    public class MethodFinderContext : IMethodFinderContext
    {
        /// <summary>
        /// Initializes the context with the default values.
        /// </summary>
        /// <param name="arguments">The list of arguments that will be passed to the target method.</param>
        public MethodFinderContext(params object[] arguments)
        {
            TypeArguments = new Type[0];
            Arguments = arguments;            
        }
        /// <summary>
        /// Initializes the context with the default values.
        /// </summary>
        /// <param name="typeArguments">The type arguments that will be used to construct the target method.</param>
        /// <param name="arguments">The list of arguments that will be passed to the target method.</param>
        /// <param name="returnType">The type that must be returned by the target method.</param>
        public MethodFinderContext(IEnumerable<Type> typeArguments, IEnumerable<object> arguments, Type returnType)
        {
            TypeArguments = typeArguments;
            Arguments = arguments;
            ReturnType = returnType;
        }

        /// <summary>
        /// Gets or sets the value indicating the type arguments that will be passed to the target method.
        /// </summary>
        public IEnumerable<Type> TypeArguments
        {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the value indicating the list of arguments that will be passed to the target method.
        /// </summary>
        public IEnumerable<object> Arguments
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="System.Type">return type</see> of the target method.
        /// </summary>
        public Type ReturnType
        {
            get; set;
        }
    }
}
