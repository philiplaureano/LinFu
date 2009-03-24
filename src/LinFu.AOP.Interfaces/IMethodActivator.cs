using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents an <see cref="IActivator{TContext}"/> that can instantiate objects from within a particular method.
    /// </summary>
    public interface IMethodActivator : IActivator<IMethodActivationContext>
    {
    }
}
