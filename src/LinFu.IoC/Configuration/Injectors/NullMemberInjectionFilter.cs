using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using System.Reflection;

namespace LinFu.IoC.Configuration.Injectors
{
    /// <summary>
    /// Represents a type of <see cref="IMemberInjectionFilter{TMember}"/>
    /// that always returns an empty match.
    /// </summary>
    public class NullMemberInjectionFilter<TMember> : IMemberInjectionFilter<TMember>
        where TMember : MemberInfo
    {
        /// <summary>
        /// Always returns an empty list of injectable members.
        /// </summary>
        /// <param name="targetType">The type to be injected.</param>
        /// <returns>An empty list.</returns>
        public IEnumerable<TMember> GetInjectableMembers(Type targetType)
        {
            return new TMember[0];
        }
    }
}
