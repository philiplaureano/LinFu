using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// An interface responsible for determining which methods
    /// should be injected.
    /// </summary>
    public interface IMemberInjectionFilter<TMember>
        where TMember : MemberInfo
    {
        /// <summary>
        /// Returns the list of <see cref="MethodBase"/> objects
        /// that will be injected with arbitrary values.
        /// </summary>
        /// <param name="targetType">The target type that contains the target methods.</param>
        /// <returns>A set of methods that describe which methods that will injected.</returns>
        IEnumerable<TMember> GetInjectableMembers(Type targetType);
    }
}
