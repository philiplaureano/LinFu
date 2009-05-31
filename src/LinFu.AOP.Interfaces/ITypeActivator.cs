using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents an <see cref="IActivator{TContext}"/> that can instantiate objects from within a particular method.
    /// </summary>
    public interface ITypeActivator : IActivator<ITypeActivationContext>
    {
        /// <summary>
        /// Determines whether or not a type can be instantiated using the 
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The <see cref="ITypeActivationContext"/> instance that describes the type to be created.</param>
        /// <returns><c>true</c> if the type can be created; otherwise, it will return <c>false</c>.</returns>
        bool CanActivate(ITypeActivationContext context);
    }
}
