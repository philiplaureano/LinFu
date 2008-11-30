using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a class that can choose a member that best matches
    /// the services currently available in a given <see cref="IServiceContainer"/> instance.
    /// </summary>
    /// <typeparam name="TMember">The member type that will be searched.</typeparam>
    public interface IMemberResolver<TMember>
        where TMember : MemberInfo
    {
        /// <summary>
        /// Uses the <paramref name="container"/> to determine which member can be used to instantiate
        /// a <paramref name="concreteType">concrete type</paramref>.
        /// </summary>
        /// <param name="concreteType">The target type.</param>
        /// <param name="container">The container that contains the service instances that will be used to invoke the target member.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to evaluate the best member for service assignment.</param>
        /// <returns>A <typeparamref name="TMember"/> instance if a match is found; otherwise, it will return <c>null</c>.</returns>
        TMember ResolveFrom(Type concreteType, IServiceContainer container,
                            params object[] additionalArguments);
    }
}
