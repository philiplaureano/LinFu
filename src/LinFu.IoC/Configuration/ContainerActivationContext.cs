using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Interfaces;
using LinFu.IoC.Configuration.Interfaces;
using ActivationContext=LinFu.AOP.Interfaces.ActivationContext;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that describes a request to instantiate a particular object type using a given
    /// <see cref="IServiceContainer"/> instance.
    /// </summary>
    public class ContainerActivationContext : ActivationContext, IContainerActivationContext
    {
        private readonly IServiceContainer _container;

        /// <summary>
        /// Initializes the class with the given parameters.
        /// </summary>
        /// <param name="concreteType">The type to be instantiated.</param>
        /// <param name="container">The container that will be used to instantiate the target type.</param>
        /// <param name="additionalArguments">The additional arguments that must be passed to the constructor.</param>
        public ContainerActivationContext(Type concreteType, IServiceContainer container, object[] additionalArguments) : base(concreteType, additionalArguments)
        {
            _container = container;    
        }

        /// <summary>
        /// Gets the value indicating the <see cref="IServiceContainer"/> instance
        /// that will instantiate the <see cref="IActivationContext.TargetType"/>.
        /// </summary>
        public IServiceContainer Container
        {
            get { return _container; }
        }
    }
}
